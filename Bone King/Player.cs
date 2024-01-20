using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class Player
    {
        public enum State
        {
            StandingRight,
            StandingLeft,
            RunningRight,
            RunningLeft,
            JumpingRight,
            JumpingLeft,
            Climbing,
            ClimbingOver,
            Death
        }

        public State state;

        Texture2D running, jumping, axe, climbing, climbingOver, death;
        Vector2 position, velocity;
        Rectangle source, feetRectangle, ladderRectangle;
        public Rectangle collision, axeCollision;

        int frameTimer, frame, deathTimer, deathStage, axeSwingTimer, axeTotalTimer, climbedLadder;
        bool facingRight;
        public bool reset, holdingAxe, axeDown;

        const int ANIMATIONSPEED = 3, LADDERANIMATIONSPEED = 8, DEATHANIMATIONSPEED = 8, AXESPEED = 15, AXETIME = 400;
        const float GRAVITY = 0.1f, MAXFALL = 3.0f, RUNSPEED = 1.5f, JUMPHEIGHT = -2.0f, JUMPSPEED = 1.5f;

        public Player(Texture2D running, Texture2D axe, Texture2D jumping, Texture2D climbing, Texture2D climbingover, Texture2D death, int x, int y)
        {

            state = State.StandingRight;

            this.running = running;
            this.axe = axe;
            this.jumping = jumping;
            this.climbing = climbing;
            climbingOver = climbingover;
            this.death = death;
            position = new Vector2(x, y);
            velocity = Vector2.Zero;
            source = new Rectangle(0, 0, 32, 32);
            feetRectangle = new Rectangle(x + 4, y + 29, 26, 3);
            ladderRectangle = new Rectangle(x + 4, y + 14, 26, 3);
            collision = new Rectangle(x + 4, y, 26, 32);
            axeCollision = new Rectangle(x - 16, y, 20, 32);

            frameTimer = ANIMATIONSPEED;
            frame = 0;
            deathTimer = 30;
            deathStage = 0;
            axeSwingTimer = AXESPEED;
            axeTotalTimer = AXETIME;

            facingRight = true;
        }

        private void StartLadderClimb(float xPos, float yPos, int ladder)
        {
            frameTimer = 0;
            state = State.Climbing;
            velocity.X = 0;
            position.X = xPos;
            position.Y = yPos;
            climbedLadder = ladder;
        }

        private void Movement(Input currentInput, State stateLeft, State stateRight, float speed)
        {
            if (currentInput.right)
            {
                velocity.X = speed;
                state = stateRight;
                facingRight = true;
            }
            else if (currentInput.left)
            {
                velocity.X = -speed;
                state = stateLeft;
                facingRight = false;
            }
            else if (facingRight)
            {
                velocity.X = 0;
                state = stateLeft;
            }
            else
            {
                velocity.X = 0;
                state = stateRight;
            }
        }

        private void Movement(Input currentInput, State stateLeft, State stateRight, State stateLeftStand, State stateRightStand, float speed)
        {
            if (currentInput.right)
            {
                velocity.X = speed;
                state = stateRight;
                facingRight = true;
            }
            else if (currentInput.left)
            {
                velocity.X = -speed;
                state = stateLeft;
                facingRight = false;
            }
            else if (facingRight)
            {
                velocity.X = 0;
                state = stateRightStand;
            }
            else
            {
                velocity.X = 0;
                state = stateLeftStand;
            }
        }

        private void SwitchTimer(ref bool switchBool, ref int timer, int resetValue)
        {
            if (timer <= 0)
            {
                if (switchBool)
                    switchBool = false;
                else
                    switchBool = true;

                timer = resetValue;
            }
            else
            {
                timer -= 1;
            }
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

        public void Update(Input currentInput, Input oldInput, Level level, GameValues gameValues, int maxX, Game1 game)
        {
            if (state == State.Death)
                return;

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

            //Running and Jumping
            if (feetCollisionCheck && !climbing)
            {
                if (currentInput.action && !holdingAxe)
                {
                    velocity.Y = JUMPHEIGHT;
                    Movement(currentInput, State.JumpingLeft, State.JumpingRight, JUMPSPEED);
                }
                else
                {
                    Movement(currentInput, State.RunningLeft, State.RunningRight, State.StandingLeft, State.StandingRight, RUNSPEED);
                }
            }

            #region Climb Ladder
            //Start climbing ladder up or down if infront of ladder and pressing correct button
            if (feetCollisionCheck && state != State.ClimbingOver && !holdingAxe)
            {
                for (int i = 0; i < level.ladders.Length; i++)
                {
                    if (currentInput.up && collision.Intersects(level.ladders[i].Body))
                    {
                        feetCollisionCheck = false;
                        StartLadderClimb(level.ladders[i].Body.Center.X - (source.Width / 2), position.Y - 1, i);
                    }
                    if (currentInput.down && feetRectangle.Intersects(level.ladders[i].Top))
                    {
                        feetCollisionCheck = false;
                        StartLadderClimb(level.ladders[i].Top.Center.X - (source.Width / 2), position.Y + 17, i);
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
                if (moving)
                    SwitchTimer(ref facingRight, ref frameTimer, LADDERANIMATIONSPEED);

                //Moves Barry down if he goes above the climbable section of any broken ladder
                if (!collision.Intersects(level.ladders[climbedLadder].Body))
                    position.Y += RUNSPEED / 2;

                //Makes Barry "Climb Over" the platform when he reaches the top of the ladder
                if (ladderRectangle.Intersects(level.ladders[climbedLadder].Top) && currentInput.up)
                {
                    state = State.ClimbingOver;
                    frame = 0;
                    frameTimer = LADDERANIMATIONSPEED;
                }
                else if (feetCollisionCheck)
                {
                    state = State.StandingLeft;
                }
            }
            #endregion

            #region Axe
            //Time the axe swing
            SwitchTimer(ref axeDown, ref axeSwingTimer, AXESPEED);

            //Stop holding axe after some time
            if (holdingAxe)
            {
                if (axeTotalTimer <= 0)
                {
                    holdingAxe = false;
                    axeTotalTimer = AXETIME;
                }
                else
                {
                    axeTotalTimer -= 1;
                }
            }
            #endregion

            //Stop Barry going off screen
            if (collision.X + collision.Width > maxX)
                position.X = maxX - collision.Width;
            if (collision.X < 0)
                position.X = 0;

            //Resets and increases level by 1 when Barry reaches goal
            if (collision.Intersects(level.goal) && !climbing && state != State.JumpingLeft && state != State.JumpingRight)
            {
                gameValues.level += 1;
                gameValues.score += (int)game.timer;

                if (gameValues.level % 5 == 0)
                    game.lives += 1;

                reset = true;
            }

            position += velocity;

            #region Sets size/position of collision rectangles depending on the Animation State
            collision.Y = (int)position.Y;
            feetRectangle.Y = (int)position.Y + 29;
            ladderRectangle.Y = (int)position.Y + 14;
            axeCollision.Y = (int)position.Y;
            source.Y = 0;
            source.Width = 32;
            source.Height = 32;

            if (state == State.StandingRight || state == State.StandingLeft)
            {
                SetColliders(4, 24, 32, 4);

                source.X = 0;

                if (holdingAxe)
                {
                    if (axeDown)
                        source.Y = 48;

                    source.Width = 48;
                    source.Height = 48;
                }
            }
            if (state == State.RunningRight || state == State.RunningLeft)
            {
                SetColliders(1, 30, 32, 1);

                if (holdingAxe)
                {
                    if (axeDown)
                        source.Y = 48;

                    if (source.X % 48 != 0)
                        source.X = 0;

                    source.Width = 48;
                    source.Height = 48;
                }
                else
                {
                    if (source.X % 32 != 0)
                        source.X = 0;
                }
            }
            if (state == State.JumpingRight || state == State.JumpingLeft)
            {
                SetColliders(2, 28, 30, 2);

                source.X = 0;
            }
            if (state == State.Climbing)
            {
                SetColliders(3, 26, 30, 3);

                source.X = 0;
            }
            if (state == State.ClimbingOver)
            {
                SetColliders(0, 32, 32, 1);

                if (source.X % 32 != 0)
                {
                    source.X = 0;
                }
            }
            #endregion
        }

        public void Reset(int x, int y)
        {
            state = State.StandingRight;

            position = new Vector2(x, y);
            velocity = Vector2.Zero;
            source = new Rectangle(0, 0, 32, 32);
            feetRectangle = new Rectangle(x + 4, y + 29, 26, 3);
            ladderRectangle = new Rectangle(x + 4, y + 14, 26, 3);
            collision = new Rectangle(x + 4, y, 26, 32);

            frameTimer = ANIMATIONSPEED;
            frame = 0;
            deathTimer = 30;
            deathStage = 0;
            axeSwingTimer = AXESPEED;
            axeTotalTimer = AXETIME;

            facingRight = true;
            reset = false;
            holdingAxe = false;
        }

        public void DeathUpdate(Game1 game)
        {
            if (game.paused == false)
            {
                if (state == State.Death)
                {
                    if (deathStage == 0)
                    {
                        if (deathTimer <= 0)
                        {
                            deathStage = 1;
                            deathTimer = 150;
                            source.X = 32;
                            if (game.deathSongInstance == null)
                            {
                                game.deathSongInstance = game.deathSong.CreateInstance();
                                game.deathSongInstance.Volume = 0.5f;
                                game.deathSongInstance.Play();
                            }
                        }
                        else
                        {
                            deathTimer -= 1;
                        }
                    }
                    if (deathStage == 1)
                    {
                        if (deathTimer <= 0)
                        {
                            deathStage = 2;
                            deathTimer = 150;
                        }
                        else
                        {
                            deathTimer -= 1;
                        }
                    }
                    if (deathStage == 2)
                    {
                        if (deathTimer <= 0)
                        {
                            reset = true;
                        }
                        else
                        {
                            deathTimer -= 1;
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch sb, Game1 game)
        {
            #region Draws based on animState
            switch (state)
            {
                case State.StandingRight:
                    if (holdingAxe)
                    {
                        sb.Draw(axe, new Vector2((int)position.X, (int)position.Y), source, Color.White, 0, new Vector2(0, 16), 1, SpriteEffects.FlipHorizontally, 0.9f);
                    }
                    else
                    {
                        sb.Draw(running, new Vector2((int)position.X, (int)position.Y), source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0.9f);
                    }
                    break;
                case State.StandingLeft:
                    if (holdingAxe)
                    {
                        sb.Draw(axe, new Vector2((int)position.X, (int)position.Y), source, Color.White, 0, new Vector2(20, 16), 1, SpriteEffects.None, 0.9f);
                    }
                    else
                    {
                        sb.Draw(running, new Vector2((int)position.X, (int)position.Y), source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    }
                    break;
                case State.RunningRight:
                    if (holdingAxe)
                    {
                        sb.Draw(axe, new Vector2((int)position.X, (int)position.Y), source, Color.White, 0, new Vector2(0, 16), 1, SpriteEffects.FlipHorizontally, 0.9f);
                    }
                    else
                    {
                        sb.Draw(running, new Vector2((int)position.X, (int)position.Y), source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0.9f);
                    }
                    if (game.paused == false)
                    {
                        if (frameTimer <= 0)
                        {
                            source.X += source.Width;
                            if (source.X >= running.Width)
                            {
                                source.X = 0;
                            }
                            frameTimer = ANIMATIONSPEED;
                        }
                        else
                        {
                            frameTimer -= 1;
                        }
                    }
                    break;
                case State.RunningLeft:
                    if (holdingAxe)
                    {
                        sb.Draw(axe, new Vector2((int)position.X, (int)position.Y), source, Color.White, 0, new Vector2(20, 16), 1, SpriteEffects.None, 0.9f);
                    }
                    else
                    {
                        sb.Draw(running, new Vector2((int)position.X, (int)position.Y), source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    }
                    if (game.paused == false)
                    {
                        if (frameTimer <= 0)
                        {
                            source.X += source.Width;
                            if (source.X >= running.Width)
                            {
                                source.X = 0;
                            }
                            frameTimer = ANIMATIONSPEED;
                        }
                        else
                        {
                            frameTimer -= 1;
                        }
                    }
                    break;
                case State.JumpingRight:
                    sb.Draw(jumping, new Vector2((int)position.X, (int)position.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0.9f);
                    break;
                case State.JumpingLeft:
                    sb.Draw(jumping, new Vector2((int)position.X, (int)position.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    break;
                case State.Climbing:
                    if (facingRight)
                    {
                        sb.Draw(climbing, new Vector2((int)position.X, (int)position.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    }
                    else
                    {
                        sb.Draw(climbing, new Vector2((int)position.X, (int)position.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0.9f);
                    }
                    break;
                case State.ClimbingOver:
                    sb.Draw(climbingOver, new Vector2((int)position.X, (int)position.Y), source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    if (game.paused == false)
                    {
                        if (frameTimer <= 0)
                        {
                            source.X += source.Width;
                            if (source.X >= climbingOver.Width)
                            {
                                frameTimer = 0;
                                source.X = 0;
                                state = State.StandingLeft;
                                frame = 0;
                            }
                            if (frame == 0 && state != State.StandingLeft)
                            {
                                position.Y -= 4;
                            }
                            if (frame == 1)
                            {
                                position.Y -= 15;
                            }
                            frame += 1;
                            frameTimer = LADDERANIMATIONSPEED;
                        }
                        else
                        {
                            frameTimer -= 1;
                        }
                    }
                    break;
                case State.Death:
                    if (deathStage == 0)
                    {
                        if (source.X % 32 != 0)
                        {
                            source.X = 0;
                        }
                        source.X = 0;
                        source.Y = 0;
                        source.Width = 32;
                        source.Height = 32;
                        sb.Draw(death, new Vector2((int)position.X, (int)position.Y), source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    }
                    if (deathStage == 1)
                    {
                        sb.Draw(death, new Vector2((int)position.X, (int)position.Y), source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                        if (game.paused == false)
                        {
                            if (frameTimer <= 0)
                            {
                                source.X += 32;
                                frameTimer = DEATHANIMATIONSPEED;
                                if (source.X >= 160)
                                {
                                    source.X = 32;
                                }
                            }
                            else
                            {
                                frameTimer -= 1;
                            }
                        }
                    }
                    if (deathStage == 2)
                    {
                        source.X = 160;
                        sb.Draw(death, new Vector2((int)position.X, (int)position.Y), source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    }
                    break;
                default:
                    //panic
                    break;
            }
            #endregion
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
