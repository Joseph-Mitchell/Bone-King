using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class Button
    {
        Texture2D m_texture;
        Vector2 m_position;
        public Color color;

        public bool hovered;

        public Button(Texture2D texture, int x, int y)
        {
            m_texture = texture;
            m_position = new Vector2(x, y);
            color = Color.Gray;
        }

        public void Draw(SpriteBatch sb)
        {
            if (hovered)
            {
                sb.Draw(m_texture, m_position, null, Color.White, 0, new Vector2(m_texture.Bounds.Center.X, m_texture.Bounds.Center.Y), 1, SpriteEffects.None, 1);
            }
            else
            {
                sb.Draw(m_texture, m_position, null, color, 0, new Vector2(m_texture.Bounds.Center.X, m_texture.Bounds.Center.Y), 1, SpriteEffects.None, 1);
            }
        }
    }
}
