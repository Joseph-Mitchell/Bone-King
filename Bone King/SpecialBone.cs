using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class SpecialBone
    {

        Texture2D spriteSheet;
        Vector2 position, velocity;
        Rectangle source;
        public Rectangle collision;

        int frameTimer;
        bool collisionCheck, collisionCheckOld;
        public bool active;

        const int ANIMATIONSPEED = 4;
        const float GRAVITY = 0.1f;

        public SpecialBone(Texture2D spritesheet, int x, int y)
        {
            spriteSheet = spritesheet;
            position = new Vector2(x, y);
            velocity = Vector2.Zero;

            source = new Rectangle(0, 0, 50, 33);

            collision = new Rectangle(x + 4, y + 9, 42, 15);

            frameTimer = ANIMATIONSPEED;
            active = true;
        }

        public void Update(GameTime gt, Level background, Game1 game)
        {
            position += velocity;
            collisionCheck = false;

            //Gravity
            if (velocity.Y < GRAVITY * 30)
            {
                velocity.Y += GRAVITY;
            }

            //Checks if the bone hits a platform
            for (int i = 0; i < background.platformHitBoxes.Length; i++)
            {
                if (collision.Intersects(background.platformHitBoxes[i]))
                {
                    collisionCheck = true;
                }
            }

            //Slows down the bone every time it hits a platform
            if (collisionCheck && collisionCheckOld == false)
            {
                velocity.Y = 0;
                game.bang.Play();
            }

            //Makes collision rectangles follow the main sprite position
            collision = new Rectangle((int)position.X + 4, (int)position.Y + 9, 42, 15);

            collisionCheckOld = collisionCheck;

            if (frameTimer <= 0)
            {
                source.X = (source.X + source.Width);
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

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(spriteSheet, new Vector2((int)position.X, (int)position.Y), source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.91f);
        }

#if DEBUG
        public void DebugDraw(SpriteBatch sb, Texture2D hitBoxTexture, SpriteFont debugFont)
        {
            sb.Draw(hitBoxTexture, collision, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
        }
#endif
    }
}
