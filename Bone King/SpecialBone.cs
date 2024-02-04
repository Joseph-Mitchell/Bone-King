using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class SpecialBone
    {
        AnimatedSprite sprite;
        Vector2 position, velocity;
        public Rectangle collision;

        bool collisionCheck, collisionCheckOld;
        public bool active;

        const int ANIMATIONSPEED = 4;
        const float GRAVITY = 0.1f, TOPSPEED = 3f;

        public SpecialBone(int x, int y, Texture2D spriteSheet)
        {
            position = new Vector2(x, y);
            velocity = Vector2.Zero;

            collision = new Rectangle(x + 4, y + 9, 42, 15);

            active = true;

            sprite = new AnimatedSprite(x, y, ANIMATIONSPEED, 0.91f, new Vector2(50, 33));
            sprite.Load(spriteSheet);
        }

        public void Update(Rectangle[] platforms, SoundEffect bang)
        {
            position += velocity;
            collisionCheck = false;

            //Gravity
            if (velocity.Y < TOPSPEED)
                velocity.Y += GRAVITY;

            //Checks if the bone hits a platform
            for (int i = 0; i < platforms.Length; i++)
            {
                if (collision.Intersects(platforms[i]))
                    collisionCheck = true;
            }

            //Slows down the bone every time it hits a platform
            if (collisionCheck && collisionCheckOld == false)
            {
                velocity.Y = 0;
                bang.Play();
            }

            collision.X = (int)position.X + 4;
            collision.Y = (int)position.Y + 9;

            collisionCheckOld = collisionCheck;

            sprite.position = position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }

#if DEBUG
        public void DebugDraw(SpriteBatch spriteBatch, Texture2D texture)
        {
            spriteBatch.Draw(texture, collision, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
        }
#endif
    }
}
