using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Bone_King
{
    class Player
    {
        public enum State
        {
            Running,
            Axe,
            Jumping,
            Climbing,
            ClimbingOver,
            Death
        }

        public State state;

        AnimatedSprite sprite;
        Vector2 position, velocity;
        Rectangle feetRectangle, ladderRectangle;
        public Rectangle collision, axeCollision;

        int frame, deathTimer, deathStage, axeSwingTimer, axeTotalTimer, climbedLadder;
        bool facingRight;
        public bool reset, holdingAxe, axeDown;

        const int ANIMATIONSPEED = 3, LADDERANIMATIONSPEED = 8, DEATHANIMATIONSPEED = 8, AXESPEED = 15, AXETIME = 400;
        const float GRAVITY = 0.1f, MAXFALL = 3.0f, RUNSPEED = 1.5f, JUMPHEIGHT = -2.0f, JUMPSPEED = 1.5f;

        public bool Jumping
        {
            get { return state == State.Jumping; }
        }
        public Player(int x, int y)
        {
            state = State.Running;

            Vector2 source1 = new Vector2(32, 32);
            Vector2 source2 = new Vector2(48, 48);
            position = new Vector2(x, y);
            sprite = new AnimatedSprite(position, ANIMATIONSPEED, 0.9f, new List<Vector2>{source1, source2, source1, source1, source1, source1});
            velocity = Vector2.Zero;
            feetRectangle = new Rectangle(x + 4, y + 29, 26, 3);
            ladderRectangle = new Rectangle(x + 4, y + 14, 26, 3);
            collision = new Rectangle(x + 4, y, 26, 32);
            axeCollision = new Rectangle(x - 16, y, 20, 32);

            frame = 0;
            deathTimer = 0;
            deathStage = 0;
            axeSwingTimer = AXESPEED;
            axeTotalTimer = AXETIME;

            facingRight = true;
        }

        public void Load(List<Texture2D> spriteSheets)
        {
            sprite.Load(spriteSheets);
        }

        public void Reset(int x, int y)
        {
            SetState(State.Running, animationSpeed: ANIMATIONSPEED, reset: true);

            position.X = x;
            position.Y = y;

            velocity = Vector2.Zero;

            frame = 0;
            deathTimer = 0;
            deathStage = 0;
            axeSwingTimer = AXESPEED;
            axeTotalTimer = AXETIME;

            facingRight = true;
            reset = false;
            holdingAxe = false;
        }

        public void GetAxe()
        {
            SetState(State.Axe, reset: true);
            holdingAxe = true;
        }

        private void StartLadderClimb(float xPos, float yPos, int ladder)
        {
            SetState(State.Climbing, animationSpeed: LADDERANIMATIONSPEED, freezeFrame: 0, reset: true);
            velocity.X = 0;
            position.X = xPos;
            position.Y = yPos;
            climbedLadder = ladder;
        }

        private void SetColliders(int X, int Width, int Height, int axeX)
        {
            collision.X = (int)position.X + X;
            collision.Width = Width;
            collision.Height = Height;

            feetRectangle.X = (int)position.X + X;
            feetRectangle.Width = Width;

            ladderRectangle.X = (int)position.X + X;
            ladderRectangle.Width = Width;

            if (facingRight)
                axeCollision.X = (int)position.X + collision.Width + axeX;
            else
                axeCollision.X = (int)position.X - (20 - axeX);
        }

        private void SetState(State state, int freezeFrame = -1, int min = 0, int max = -1, int animationSpeed = -1, bool reset = false)
        {
            this.state = state;
            SpriteEffects spriteEffects = facingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            if (freezeFrame == -1)
                sprite.ChangeSheet((int)state, min: min, max: max, animationSpeed: animationSpeed, reset: reset, spriteEffects: spriteEffects);
            else
                sprite.ChangeSheet((int)state, min: freezeFrame, max: freezeFrame + 1, animationSpeed: animationSpeed, reset: reset, spriteEffects: spriteEffects);
        }

        public void Update(Input currentInput, Input oldInput, Level level, GameValues gameValues, int maxX, Game1 game)
        {
            if (state == State.Death)
                return;

            #region Sets size/position of collision rectangles depending on the Animation State
            collision.Y = (int)position.Y;
            feetRectangle.Y = (int)position.Y + 29;
            ladderRectangle.Y = (int)position.Y + 14;
            axeCollision.Y = (int)position.Y;

            if (state == State.Running || state == State.Axe)
                SetColliders(1, 30, 32, 1);
            else if (state == State.Jumping)
                SetColliders(2, 28, 30, 2);
            else if (state == State.Climbing)
                SetColliders(3, 26, 30, 3);
            else if (state == State.ClimbingOver)
                SetColliders(0, 32, 32, 1);
            #endregion

            bool climbing = state == State.Climbing || state == State.ClimbingOver;

            //Gravity
            if (velocity.Y < MAXFALL && !climbing)
                velocity.Y += GRAVITY;

            //Checks if player is touching the ground
            bool feetCollisionCheck = false;
            foreach (Rectangle platform in level.platformHitBoxes)
            {
                if (!platform.Intersects(feetRectangle))
                    continue;

                feetCollisionCheck = true;

                if (!climbing)
                { 
                    if (velocity.Y > 0)
                        velocity.Y = 0;

                    if (position.Y > platform.Top - collision.Height + 1)
                        position.Y = platform.Top - collision.Height + 1;
                }
            }

            #region Running and Jumping
            if (feetCollisionCheck && !climbing)
            {
                if (currentInput.action && !holdingAxe)
                {
                    velocity.Y = JUMPHEIGHT;
                    if (currentInput.right)
                    {
                        velocity.X = JUMPSPEED;
                        facingRight = true;
                        SetState(State.Jumping, reset: true);
                    }
                    else if (currentInput.left)
                    {
                        velocity.X = -JUMPSPEED;
                        facingRight = false;
                        SetState(State.Jumping, reset: true);
                    }
                    else
                    {
                        velocity.X = 0;
                        SetState(State.Jumping, freezeFrame: 0, reset: true);
                    }
                }
                else
                {
                    State newState = state == State.Axe ? State.Axe : State.Running;
                    if (currentInput.right)
                    {
                        velocity.X = RUNSPEED;
                        facingRight = true;
                        SetState(newState);
                    }
                    else if (currentInput.left)
                    {
                        velocity.X = -RUNSPEED;
                        facingRight = false;
                        SetState(newState);
                    }
                    else
                    {
                        velocity.X = 0;
                        SetState(newState, freezeFrame: 0);
                    }
                }
            }
            #endregion

            #region Climb Ladder
            //Start climbing ladder up or down if infront of ladder and pressing correct button
            if (feetCollisionCheck && state != State.ClimbingOver && !holdingAxe)
            {
                for (int i = 0; i < level.ladders.Length; i++)
                {
                    if (currentInput.up && collision.Intersects(level.ladders[i].Body))
                    {
                        feetCollisionCheck = false;
                        StartLadderClimb(level.ladders[i].Body.Center.X - 16, position.Y - 1, i);
                    }
                    if (currentInput.down && feetRectangle.Intersects(level.ladders[i].Top) && !level.ladders[i].Broken)
                    {
                        feetCollisionCheck = false;
                        StartLadderClimb(level.ladders[i].Top.Center.X - 16, position.Y + 17, i);
                    }
                }
            }

            //Moves player on the ladder
            if (state == State.Climbing)
            {
                bool moving = false;

                if (currentInput.up)
                {
                    position.Y -= RUNSPEED / 2;
                    moving = true;
                }
                if (currentInput.down)
                {
                    position.Y += RUNSPEED / 2;
                    moving = true;
                }

                //Flips sprite left and right while climbing
                if (moving && sprite.FrameFlag)
                {
                    if (facingRight)
                    {
                        facingRight = false;
                        SetState(State.Climbing);
                    }
                    else
                    {
                        facingRight = true;
                        SetState(State.Climbing);
                    }
                }

                //Moves Barry down if he goes above the climbable section of any broken ladder
                if (!collision.Intersects(level.ladders[climbedLadder].Body))
                    position.Y += RUNSPEED / 2;

                //Makes Barry "Climb Over" the platform when he reaches the top of the ladder
                if (ladderRectangle.Intersects(level.ladders[climbedLadder].Top) && currentInput.up)
                {
                    frame = 0;
                    SetState(State.ClimbingOver, reset: true);
                }
                else if (feetCollisionCheck)
                {
                    SetState(State.Running, animationSpeed: ANIMATIONSPEED, reset: true);
                }
            }

            if (state == State.ClimbingOver && sprite.FrameFlag)
            {
                if (frame == 0)
                {
                    position.Y -= 4;
                }
                else if (frame == 1)
                {
                    position.Y -= 15;
                }
                else if (frame == 2)
                {
                    SetState(State.Running, freezeFrame: 0, animationSpeed: ANIMATIONSPEED, reset: true);
                }
                frame++;
            }
            #endregion

            #region Axe
            if (holdingAxe)
            {
                if (axeSwingTimer > 0)
                {
                    axeSwingTimer -= 1;
                }
                else
                {
                    if (axeDown)
                    {
                        axeDown = false;
                        sprite.YLayer = 0;
                    }
                    else
                    {
                        axeDown = true;
                        sprite.YLayer = 1;
                    }

                    axeSwingTimer = AXESPEED;
                }

                if (axeTotalTimer > 0)
                {
                    axeTotalTimer -= 1;
                }
                else
                {
                    sprite.YLayer = 0;
                    axeDown = false;
                    holdingAxe = false;
                    axeTotalTimer = AXETIME;
                    SetState(State.Running, reset: true);
                }
            }
            #endregion

            //Stop Player going off screen
            if (collision.X + collision.Width > maxX)
                position.X = maxX - collision.Width;
            if (collision.X < 0)
                position.X = 0;

            //Resets and increases level by 1 when Player reaches goal
            if (collision.Intersects(level.goal) && !climbing && state != State.Jumping)
            {
                gameValues.level += 1;
                gameValues.score += (int)game.timer;

                if (gameValues.level % 5 == 0)
                    game.lives += 1;

                reset = true;
            }

            position += velocity;

            //Set sprite position
            if (state == State.Axe)
            {
                if (facingRight)
                    sprite.position.X = position.X;
                else
                    sprite.position.X = position.X - 16;

                sprite.position.Y = position.Y - 16;
            }
            else
            {
                sprite.position.X = position.X;
                sprite.position.Y = position.Y;
            }
        }

        public void DeathUpdate(Game1 game)
        {
            if (game.paused || state != State.Death)
                return;

            if (deathTimer > 0)
            {
                deathTimer--;
                return;
            }

            switch (deathStage)
            {
                case 0:

                    SetState(State.Death, freezeFrame: 0, animationSpeed: DEATHANIMATIONSPEED, reset: true);

                    deathStage = 1;
                    deathTimer = 30;

                    break;
                case 1:

                    SetState(State.Death, min: 1, max: 4, animationSpeed: DEATHANIMATIONSPEED, reset: false);

                    deathStage = 2;
                    deathTimer = 160;

                    if (game.deathSongInstance == null)
                    {
                        game.deathSongInstance = game.deathSong.CreateInstance();
                        game.deathSongInstance.Volume = 0.5f;
                        game.deathSongInstance.Play();
                    }
                    
                    break;
                case 2:

                    SetState(State.Death, freezeFrame: 5, animationSpeed: DEATHANIMATIONSPEED, reset: true);

                    deathStage = 3;
                    deathTimer = 150;
                    
                    break;
                case 3:

                    reset = true;

                    break;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }
#if DEBUG
        public void DebugDraw(SpriteBatch sb, Texture2D hitBox, SpriteFont font)
        {
            sb.Draw(hitBox, collision, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
            sb.Draw(hitBox, feetRectangle, null, Color.Blue, 0, Vector2.Zero, SpriteEffects.None, 1);
            sb.Draw(hitBox, ladderRectangle, null, Color.Yellow, 0, Vector2.Zero, SpriteEffects.None, 1);
            sb.Draw(hitBox, axeCollision, null, Color.Green, 0, Vector2.Zero, SpriteEffects.None, 1f);
            sb.DrawString(font, "Axe Time: " + axeTotalTimer, new Vector2(0, 16), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
        }
#endif
    }
}
