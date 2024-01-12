using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Bone_King
{
    class Button
    {
        Texture2D texture;
        Vector2 position;

        public ButtonEffect effect;

        public bool hovered, active;

        public Button(Texture2D texture, int x, int y, ButtonEffect effect)
        {
            active = true;
            this.texture = texture;
            position = new Vector2(x, y);
            this.effect = effect;
        }

        public void Draw(SpriteBatch sb)
        {
            if (!active)
            {
                sb.Draw(texture, position, null, Color.White * 0.5f, 0, new Vector2(texture.Bounds.Center.X, texture.Bounds.Center.Y), 1, SpriteEffects.None, 1);
            }
            else if (hovered)
            {
                sb.Draw(texture, position, null, Color.White, 0, new Vector2(texture.Bounds.Center.X, texture.Bounds.Center.Y), 1, SpriteEffects.None, 1);
            }
            else
            {
                sb.Draw(texture, position, null, Color.Gray, 0, new Vector2(texture.Bounds.Center.X, texture.Bounds.Center.Y), 1, SpriteEffects.None, 1);
            }
        }
    }
}
