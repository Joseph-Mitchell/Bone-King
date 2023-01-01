using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class StaticObject
    {
        Texture2D m_texture;
        public Vector2 m_position;

        float m_layer;
        public StaticObject(Texture2D texture, int x, int y, float layer)
        {
            m_texture = texture;
            m_position = new Vector2(x, y);
            m_layer = layer;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(m_texture, m_position, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, m_layer);
        }
    }
}
