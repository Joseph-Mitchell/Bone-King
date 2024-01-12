using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class Button
    {
        Texture2D texture;
        Vector2 position;
        public Color color;

        public bool hovered;

        public Button(Texture2D texture, int x, int y)
        {
            this.texture = texture;
            position = new Vector2(x, y);
            color = Color.Gray;
        }

        public void Draw(SpriteBatch sb)
        {
            if (hovered)
            {
                sb.Draw(texture, position, null, Color.White, 0, new Vector2(texture.Bounds.Center.X, texture.Bounds.Center.Y), 1, SpriteEffects.None, 1);
            }
            else
            {
                sb.Draw(texture, position, null, color, 0, new Vector2(texture.Bounds.Center.X, texture.Bounds.Center.Y), 1, SpriteEffects.None, 1);
            }
        }
    }
}
