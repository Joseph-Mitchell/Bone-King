using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Bone_King
{
    class Bone
    {
        public enum State
        {
            None,
            RollingRight,
            RollingLeft,
            FallingRight,
            FallingLeft,
            Ladder
        }

        Texture2D normalSprites, ladderSprites;
        Vector2 position, velocity;
        Rectangle source;
        public Rectangle groundCollision, playerCollision, scoreRectangle;

        public State animState;

        float frameTimer;
        bool collisionCheck, ladderCheck;
        public bool isActive;

        const int ANIMATIONSPEED = 4;
        const float GRAVITY = 0.1f, BONESPEED = 1.5f;

        public Bone(Texture2D normal, Texture2D ladder, GameValues level, int x, int y)
        {
            normalSprites = normal;
            ladderSprites = ladder;
            position = new Vector2(x, y);
            velocity = Vector2.Zero;

            source = new Rectangle(0, 0, 33, 33);

            groundCollision = new Rectangle(x - 7, y - 7, 15, 15);
            playerCollision = new Rectangle(x - 7, y - 7, 15, 15);
            scoreRectangle = new Rectangle(x - 2, y - 17, 4, 10);

            animState = State.RollingRight;

            frameTimer = ANIMATIONSPEED;
            isActive = true;
        }

        public void Update(GameTime gt, Background background, Random RNG, GameValues level)
        {
            position += velocity;
            collisionCheck = false;

            //Checks if the bone is on any of the platforms or ladders
            for (int i = 0; i < background.ladderTops.Length; i++)
            {
                if (groundCollision.Intersects(background.ladderTops[i]) && groundCollision.X + (groundCollision.Width / 2) > background.ladderTops[i].X + (background.ladderTops[i].Width / 3) && groundCollision.X + (groundCollision.Width / 2) < background.ladderTops[i].X + ((background.ladderTops[i].Width / 3) * 2) && ladderCheck == false)
                {
                    int potato = RNG.Next(0, 8);
                    if (potato == 0)
                    {
                        ladderCheck = true;
                        position.X = background.ladderTops[i].X + (background.ladderTops[i].Width / 2);
                        position.Y += 10;
                    }
                }
            }
            for (int i = 0; i < background.platformHitBoxes.Length; i++)
            {
                if (groundCollision.Intersects(background.platformHitBoxes[i]) && ladderCheck == false)
                {
                    collisionCheck = true;
                    position.Y = background.platformHitBoxes[i].Y - 7;
                }
            }

            //Checks if the bone is at the bottom of a ladder
            if (ladderCheck)
            {
                ladderCheck = false;
                for (int i = 0; i < background.ladderTops.Length; i++)
                {
                    if (groundCollision.Intersects(background.ladderTops[i]))
                    {
                        ladderCheck = true;
                    }
                }
            }

            //Changes state depending on bones position and collisions
            if (collisionCheck && ladderCheck == false && animState != State.Ladder)
            {
                if ((position.Y >= 155 && position.Y <= 213) || (position.Y >= 275 && position.Y <= 329) || position.Y >= 397)
                {
                    animState = State.RollingLeft;
                }
                else
                {
                    animState = State.RollingRight;
                }
            }
            else if (ladderCheck == false && animState != State.Ladder)
            {
                if ((position.Y >= 155 && position.Y <= 213) || (position.Y >= 275 && position.Y <= 329) || position.Y >= 397)
                {
                    animState = State.FallingLeft;
                }
                else if (velocity.X > 0)
                {
                    animState = State.FallingRight;
                }
            }
            else
            {
                animState = State.Ladder;
            }

            //Times the animation
            if (frameTimer <= 0)
            {
                source.X = (source.X + source.Width);

                if (animState == State.Ladder)
                {
                    if (source.X >= ladderSprites.Width)
                    {
                        source.X = 0;
                    }
                }
                else
                {
                    if (source.X >= normalSprites.Width)
                    {
                        source.X = 0;
                    }
                }

                frameTimer = ANIMATIONSPEED / level.multiplier;
            }
            else
            {
                frameTimer -= 1;
            }

            //Changes values depending on state
            switch (animState)
            {
                case State.RollingLeft:
                    velocity.X = -(BONESPEED * level.multiplier);
                    velocity.Y = 0;
                    source.Width = 33;
                    source.Height = 33;
                    if (source.X % 33 != 0)
                    {
                        source.X = 0;
                    }
                    groundCollision.X = (int)position.X - 7;
                    groundCollision.Y = (int)position.Y - 7;
                    playerCollision = new Rectangle((int)position.X - 7, (int)position.Y - 5, 15, 13);
                    scoreRectangle = new Rectangle((int)position.X - 2, (int)position.Y - 27, 4, 20);
                    break;
                case State.RollingRight:
                    velocity.X = BONESPEED * level.multiplier;
                    velocity.Y = 0;
                    source.Width = 33;
                    source.Height = 33;
                    if (source.X % 33 != 0)
                    {
                        source.X = 0;
                    }
                    groundCollision.X = (int)position.X - 7;
                    groundCollision.Y = (int)position.Y - 7;
                    playerCollision = new Rectangle((int)position.X - 7, (int)position.Y - 5, 15, 13);
                    scoreRectangle = new Rectangle((int)position.X - 2, (int)position.Y - 27, 4, 20);
                    break;
                case State.FallingLeft:
                    if (position.X + (groundCollision.Width / 2) < 64)
                    {
                        velocity.X = -(BONESPEED * level.multiplier) / 2;
                    }
                    else
                    {
                        velocity.X = -(BONESPEED * level.multiplier);
                    }
                    velocity.Y += GRAVITY * level.multiplier;
                    source.Width = 33;
                    source.Height = 33;
                    if (source.X % 33 != 0)
                    {
                        source.X = 0;
                    }
                    groundCollision.X = (int)position.X - 7;
                    groundCollision.Y = (int)position.Y - 7;
                    playerCollision = new Rectangle((int)position.X - 7, (int)position.Y - 5, 15, 13);
                    scoreRectangle = new Rectangle((int)position.X - 2, (int)position.Y - 27, 4, 20);
                    break;
                case State.FallingRight:
                    if (position.X - (groundCollision.Width / 2) > 448)
                    {
                        velocity.X = (BONESPEED * level.multiplier) / 2;
                    }
                    else
                    {
                        velocity.X = BONESPEED * level.multiplier;
                    }
                    velocity.Y += GRAVITY * level.multiplier;
                    source.Width = 33;
                    source.Height = 33;
                    if (source.X % 33 != 0)
                    {
                        source.X = 0;
                    }
                    groundCollision.X = (int)position.X - 7;
                    groundCollision.Y = (int)position.Y - 7;
                    playerCollision = new Rectangle((int)position.X - 7, (int)position.Y - 5, 15, 13);
                    scoreRectangle = new Rectangle((int)position.X - 2, (int)position.Y - 27, 4, 20);
                    break;
                case State.Ladder:
                    velocity.X = 0;
                    velocity.Y = (BONESPEED * level.multiplier) * 0.6f;
                    if (collisionCheck)
                    {
                        animState = State.None;
                    }
                    source.Width = 50;
                    source.Height = 33;
                    if (source.X % 50 != 0)
                    {
                        source.X = 0;
                    }
                    groundCollision.X = (int)position.X - 7;
                    groundCollision.Y = (int)position.Y - 7;
                    playerCollision = new Rectangle((int)position.X - 20, (int)position.Y - 7, 40, 15);
                    scoreRectangle = new Rectangle((int)position.X - 2, (int)position.Y - 27, 4, 20);
                    break;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            //Chooses which spritesheet to use, and whether to flip it
            if (animState == State.FallingLeft || animState == State.RollingLeft)
            {
                sb.Draw(normalSprites, new Vector2((int)position.X, (int)position.Y), source, Color.White, 0, new Vector2(source.Width / 2, source.Height / 2), 1, SpriteEffects.FlipHorizontally, 0.9f);
            }
            else if (animState == State.FallingRight || animState == State.RollingRight)
            {
                sb.Draw(normalSprites, new Vector2((int)position.X, (int)position.Y), source, Color.White, 0, new Vector2(source.Width / 2, source.Height / 2), 1, SpriteEffects.None, 0.9f);
            }
            else if (animState == State.Ladder)
            {
                sb.Draw(ladderSprites, new Vector2((int)position.X, (int)position.Y), source, Color.White, 0, new Vector2(source.Width / 2, source.Height / 2), 1, SpriteEffects.None, 0.9f);
            }
        }
#if DEBUG
        public void DebugDraw(SpriteBatch sb, Texture2D hitBoxTexture, SpriteFont debugFont)
        {
            sb.Draw(hitBoxTexture, groundCollision, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            sb.Draw(hitBoxTexture, playerCollision, null, Color.Blue, 0, Vector2.Zero, SpriteEffects.None, 0.99f);
            sb.Draw(hitBoxTexture, scoreRectangle, null, Color.Yellow, 0, Vector2.Zero, SpriteEffects.None, 1);
        }
#endif
    }
}
