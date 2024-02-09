using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    public class Particle
    {
        Sprite sprite;
        public Vector2 velocity;
        public float rotationSpeed;
        public int lifeTime;

        const float GRAVITY = 0.15f;
        public Particle(Texture2D texture, Vector2 position, Vector2 velocity, Color color, float rotation, float rotationSpeed, int lifeTime)
        {
            sprite = new Sprite(position, 1)
            {
                centerOrigin = true,
                rotation = rotation,
                color = color
            };

            this.velocity = velocity;
            this.rotationSpeed = rotationSpeed;
            this.lifeTime = lifeTime;

            sprite.Load(texture);
        }

        public void Update()
        {
            if (velocity.Y < GRAVITY * 180)
            {
                velocity.Y += GRAVITY;
            }

            lifeTime--;
            sprite.position += velocity;
            sprite.rotation += rotationSpeed;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }
    }
}
