using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.CompilerServices;

namespace Bone_King
{
    class SpecialBone : PhysicsObject
    {
        AnimatedSprite sprite;
        public bool active;

        const int ANIMATIONSPEED = 4;
        const float TOPSPEED = 3f;

        public Rectangle Collision
        {
            get
            {
                return colliders[0].Area;
            }
        }

        public SpecialBone(Vector2 position, Texture2D spriteSheet):base(position)
        {
            colliders[0].Area = new Rectangle((int)position.X, (int)position.Y, 42, 15);
            colliders[0].Offset = new Vector2(4, 9);

            active = true;

            sprite = new AnimatedSprite(position, ANIMATIONSPEED, 0.91f, new Vector2(50, 33));
            sprite.Load(spriteSheet);
        }

        protected sealed override void EndUpdate()
        {
            base.EndUpdate();
            sprite.Update(position);
        }

        public void Update(Rectangle[] platforms, SoundEffect bang)
        {
            UpdatePosition();

            CheckGrounded(platforms);

            if (velocity.Y < TOPSPEED)
                velocity.Y += GRAVITY;

            //Slows down the bone every time it hits a platform
            if (grounded && !groundedOld)
            {
                velocity.Y = 0;
                bang.Play();
            }

            UpdateColliders();

            EndUpdate();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }

#if DEBUG
        public void DebugDraw(SpriteBatch spriteBatch, Texture2D texture)
        {
            spriteBatch.Draw(texture, colliders[0].Area, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
        }
#endif
    }
}
