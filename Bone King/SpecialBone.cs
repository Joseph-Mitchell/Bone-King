using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class SpecialBone : PhysicsObject
    {
        AnimatedSprite sprite;
        public bool active;

        const int ANIMATIONSPEED = 4;

        protected override bool DontGround => true;

        public Rectangle Collision => colliders[0].Area;

        public SpecialBone(Vector2 position, Texture2D spriteSheet) : base(position, new Point(42, 15), new Point(4, 9))
        {
            active = true;

            sprite = new AnimatedSprite(position, ANIMATIONSPEED, 0.91f, new Vector2(50, 33));
            sprite.Load(spriteSheet);
        }

        protected override void UpdatePosition()
        {
            base.UpdatePosition();
            sprite.Update(position);
        }

        protected override void Gravity()
        {
            if (velocity.Y < MAXFALL)
                velocity.Y += GRAVITY;
        }

        public void Update(Rectangle[] platforms, SoundEffect bang)
        {
            UpdatePosition();

            bool groundedOld = grounded;
            CheckGrounded(platforms);

            Gravity();

            //Slows down the bone every time it hits a platform
            if (grounded && !groundedOld)
            {
                velocity.Y = 0;
                bang.Play();
            }

            UpdateColliders();
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
