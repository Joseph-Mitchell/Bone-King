using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Bone_King
{
    class AnimatedSprite
    {
        List<Texture2D> spriteSheets;
        Vector2 position;
        Rectangle source;

        int frameTimer, animationSpeed, currentSpriteSheet;
        float layer;

        public AnimatedSprite (int x, int y, int width, int height, int animationSpeed, float layer)
        {
            position = new Vector2(x, y);
            source = new Rectangle(0, 0, width, height);

            frameTimer = animationSpeed;
            this.animationSpeed = animationSpeed;
            this.layer = layer;
        }

        public void Load (List<Texture2D> spriteSheets)
        {
            this.spriteSheets = spriteSheets;
        }

        public void Draw (SpriteBatch sb)
        {
            if (frameTimer > 0)
            {
                frameTimer -= 1;
            }
            else
            {
                source.X += source.Width;
                if (source.X >= spriteSheets[currentSpriteSheet].Width)
                    source.X = 0;

                frameTimer = animationSpeed;
            }

            sb.Draw(spriteSheets[currentSpriteSheet], new Vector2((int)position.X, (int)position.Y), source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, layer);
        }
    }
}
