using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    public class Particle
    {
        public Texture2D texture;
        public Vector2 position, velocity;
        public Color color;

        public float rotation, rotationSpeed, size;
        public int lifeTime;

        const float GRAVITY = 0.15f;
        public Particle(Texture2D particleTexture, float x, float y, float xV, float yV, Color particleColor, float particleRotation, float particleRotationSpeed, float particleSize, int particleLifeTime)
        {
            texture = particleTexture;
            position = new Vector2(x, y);
            velocity = new Vector2(xV, yV);
            color = particleColor;
            rotation = particleRotation;
            rotationSpeed = particleRotationSpeed;
            size = particleSize;
            lifeTime = particleLifeTime;
        }

        public void Update()
        {
            if (velocity.Y < GRAVITY * 180)
            {
                velocity.Y += GRAVITY;
            }

            lifeTime--;
            position += velocity;
            rotation += rotationSpeed;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, position, null, color, rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1, SpriteEffects.None, 1);
        }
    }
}
