using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class AnimatedObject
    {
        Texture2D m_spriteSheet;
        Vector2 m_position;
        Rectangle m_source;

        int m_frameTimer, m_animationSpeed;
        float m_layer;

        public AnimatedObject (Texture2D spriteSheet, int x, int y, int width, int height, int animationSpeed, float layer)
        {
            m_spriteSheet = spriteSheet;
            m_position = new Vector2(x, y);
            m_source = new Rectangle(0, 0, width, height);

            m_frameTimer = animationSpeed;
            m_animationSpeed = animationSpeed;
            m_layer = layer;
        }

        public void UpdateAnimation()
        {
            if (m_frameTimer <= 0)
            {
                m_source.X += m_source.Width;
                if (m_source.X >= m_spriteSheet.Width)
                {
                    m_source.X = 0;
                }
                m_frameTimer = m_animationSpeed;
            }
            else
            {
                m_frameTimer -= 1;
            }
        }

        public void Draw (SpriteBatch sb)
        {
            sb.Draw(m_spriteSheet, new Vector2((int)m_position.X, (int)m_position.Y), m_source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, m_layer);
        }
    }
}
