using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class Skull
    {
        enum State
        {
            Ladder,
            Moving,
        }

        AnimatedSprite sprite;
        Vector2 position, velocity;
        public Rectangle collision;

        State state;

        bool vulnerable, ladderIntersect, facingRight;
        public bool active;

        const int ANIMATIONSPEED = 15;
        const float GRAVITY = 0.1f, MOVEMENTSPEED = 0.7f;

        public Skull (int x, int y, Texture2D spriteSheet)
        {
            position = new Vector2(x, y);
            sprite = new AnimatedSprite(position, ANIMATIONSPEED, 1, new Vector2(30, 32));
            collision = new Rectangle(x, y, 30, 32);

            facingRight = true;
            SetState(State.Moving);
            active = true;

            sprite.Load(spriteSheet);
        }

        public void Load(Texture2D spriteSheet)
        {
            sprite.Load(spriteSheet);
        }

        private void SetState(State newState)
        {
            state = newState;

            SpriteEffects spriteEffects = facingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            sprite.ChangeSheet(0, spriteEffects: spriteEffects);
        }

        public void Update(Level background, int maxX, Player player)
        {
            //Gravity
            if (velocity.Y < GRAVITY * 30 && state != State.Ladder)
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
            for (int i = 0; i < background.ladders.Length; i++)
            {
                if (i != 0 && i != 2 && i != 4 && i != 5 && i != 7 && i != 10)
                {
                    if (collision.Intersects(background.ladders[i].Body) && collision.X + (collision.Width / 2) > background.ladders[i].Body.X + (background.ladders[i].Body.Width / 3) && collision.X + (collision.Width / 2) < background.ladders[i].Body.X + ((background.ladders[i].Body.Width / 3) * 2) && state != State.Ladder)
                    {
                        state = State.Ladder;
                        position.X = background.ladders[i].Body.X + (background.ladders[i].Body.Width / 2) - (collision.Width / 2);
                    }
                }
                if (collision.Intersects(background.ladders[i].Body))
                {
                    ladderIntersect = true;
                }
            }

            //Moves depending on state
            if (state == State.Ladder && !ladderIntersect)
            {
                if ((position.Y >= 359 || (position.Y >= 239 && position.Y < 299) || (position.Y >= 119 && position.Y < 179)))
                    facingRight = true;
                else if (((position.Y < 359 && position.Y >= 299) || (position.Y < 239 && position.Y >= 179) || (position.Y < 119)))
                    facingRight = false;

                SetState(State.Moving);
            }

            //Movement
            if (state == State.Moving)
            {
                if (facingRight)
                    velocity.X = MOVEMENTSPEED;
                else
                    velocity.X = -MOVEMENTSPEED;
            }
            else if (state == State.Ladder)
            {
                velocity.X = 0;
                velocity.Y = -MOVEMENTSPEED;
            }

            //Stops the skull from going off the screen
            if (collision.X + collision.Width > maxX)
                position.X -= MOVEMENTSPEED;
            if (collision.X < 0)
                position.X += MOVEMENTSPEED;

            //Changes the sprites for when Barry has the axe
            if (player.holdingAxe)
                vulnerable = true;
            else
                vulnerable = false;

            if (vulnerable)
                sprite.YLayer = 1;
            else
                sprite.YLayer = 0;

            collision.X = (int)position.X;
            collision.Y = (int)position.Y;

            position += velocity;
            sprite.position = position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }
#if DEBUG
        public void DebugDraw(SpriteBatch sb, Texture2D texture)
        {
            sb.Draw(texture, collision, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
        }
#endif
    }
}
