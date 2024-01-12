using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Bone_King
{
    class Skull
    {
        enum AnimState
        {
            Ladder,
            MovingLeft,
            MovingRight,
            None
        }

        Texture2D spriteSheet;
        Vector2 position, velocity;
        Rectangle source;
        public Rectangle collision;

        AnimState animstate;

        int frameTimer;
        bool vulnerable, ladderIntersect;
        public bool isActive;

        const int ANIMATIONSPEED = 15;
        const float GRAVITY = 0.1f, MOVEMENTSPEED = 0.7f;

        public Skull (Texture2D spriteSheet, int x, int y)
        {
            this.spriteSheet = spriteSheet;
            position = new Vector2(x, y);
            source = new Rectangle(0, 0, 30, 32);
            collision = new Rectangle(x, y, 30, 32);

            animstate = AnimState.MovingRight;

            frameTimer = ANIMATIONSPEED;
            isActive = true;
        }

        public void Update(Background background, Random RNG, int maxX, Barry player)
        {
            //Gravity
            if (velocity.Y < GRAVITY * 30 && animstate != AnimState.Ladder)
            {
                velocity.Y += GRAVITY;
            }

            //Checks for collision with platforms and stops skull from falling through the ground
            for (int i = 0; i < background.platformHitBoxes.Length; i++)
            {
                if (background.platformHitBoxes[i].Intersects(collision))
                {
                    if (velocity.Y >= 0)
                    {
                        velocity.Y = 0;
                        position.Y = background.platformHitBoxes[i].Top - collision.Height + 1;
                    }
                }
            }

            //Chooses movement
            ladderIntersect = false;
            for (int i = 0; i < background.ladderHitBoxes.Length; i++)
            {
                if (i != 0 && i != 2 && i != 4 && i != 5 && i != 7 && i != 10)
                {
                    if (collision.Intersects(background.ladderHitBoxes[i]) && collision.X + (collision.Width / 2) > background.ladderHitBoxes[i].X + (background.ladderHitBoxes[i].Width / 3) && collision.X + (collision.Width / 2) < background.ladderHitBoxes[i].X + ((background.ladderHitBoxes[i].Width / 3) * 2) && animstate != AnimState.Ladder)
                    {
                        animstate = AnimState.Ladder;
                        position.X = background.ladderHitBoxes[i].X + (background.ladderHitBoxes[i].Width / 2) - (collision.Width / 2);
                    }
                }
                if (collision.Intersects(background.ladderHitBoxes[i]))
                {
                    ladderIntersect = true;
                }
            }

            //Moves depending on animstate
            if (animstate == AnimState.Ladder && !ladderIntersect)
            {
                animstate = AnimState.None;
            }
            if ((position.Y >= 359 || (position.Y >= 239 && position.Y < 299) || (position.Y >= 119 && position.Y < 179)) && animstate != AnimState.Ladder)
            {
                animstate = AnimState.MovingRight;
            }
            else if (((position.Y < 359 && position.Y >= 299) || (position.Y < 239 && position.Y >= 179) || (position.Y < 119)) && animstate != AnimState.Ladder)
            {
                animstate = AnimState.MovingLeft;
            }

            //Movement
            if (animstate == AnimState.MovingLeft)
            {
                velocity.X = -MOVEMENTSPEED;
            }
            else if (animstate == AnimState.MovingRight)
            {
                velocity.X = MOVEMENTSPEED;
            }
            else if (animstate == AnimState.Ladder)
            {
                velocity.X = 0;
                velocity.Y = -MOVEMENTSPEED;
            }
            else if (animstate == AnimState.None)
            {
                velocity.X = 0;
                velocity.Y = 0;
            }

            //Stops the skull from going off the screen
            if (collision.X + collision.Width > maxX)
            {
                position.X -= MOVEMENTSPEED;
            }
            if (collision.X < 0)
            {
                position.X += MOVEMENTSPEED;
            }

            //Changes the sprites for when Barry has the axe
            if (player.holdingAxe)
            {
                vulnerable = true;
            }
            else
            {
                vulnerable = false;
            }
            if (vulnerable)
            {
                source.Y = 32;
            }
            else
            {
                source.Y = 0;
            }

            collision.X = (int)position.X;
            collision.Y = (int)position.Y;

            position += velocity;
        }

        public void Draw(SpriteBatch sb, Game1 game)
        {
            if (game.paused == false)
            {
                if (frameTimer <= 0)
                {
                    source.X += source.Width;
                    if (source.X >= spriteSheet.Width)
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
            if (animstate == AnimState.MovingRight)
            {
                sb.Draw(spriteSheet, new Vector2((int)position.X, (int)position.Y), source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0.8f);
            }
            else
            {
                sb.Draw(spriteSheet, new Vector2((int)position.X, (int)position.Y), source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.8f);
            }
        }
#if DEBUG
        public void DebugDraw(SpriteBatch sb, Texture2D texture)
        {
            sb.Draw(texture, collision, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
        }
#endif
    }
}
