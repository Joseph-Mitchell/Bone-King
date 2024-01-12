using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class AnimatedSprite
    {
        Texture2D spriteSheet;
        Vector2 position;
        Rectangle source;

        int frameTimer, animationSpeed;
        float layer;

        public AnimatedSprite (Texture2D spriteSheet, int x, int y, int width, int height, int animationSpeed, float layer)
        {
            this.spriteSheet = spriteSheet;
            position = new Vector2(x, y);
            source = new Rectangle(0, 0, width, height);

            frameTimer = animationSpeed;
            this.animationSpeed = animationSpeed;
            this.layer = layer;
        }

        public void UpdateAnimation()
        {
            if (frameTimer <= 0)
            {
                source.X += source.Width;
                if (source.X >= spriteSheet.Width)
                {
                    source.X = 0;
                }
                frameTimer = animationSpeed;
            }
            else
            {
                frameTimer -= 1;
            }
        }

        public void Draw (SpriteBatch sb)
        {
            sb.Draw(spriteSheet, new Vector2((int)position.X, (int)position.Y), source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, layer);
        }
    }
}
