using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class Scores
    {
        Texture2D m_texture;
        Vector2 m_position;

        float m_frameTimer;
        public bool isActive;

        const float ACTIVETIME = 60;
        public Scores(Texture2D texture, int x, int y)
        {
            m_texture = texture;
            m_position = new Vector2(x, y);

            isActive = true;
            m_frameTimer = ACTIVETIME;
        }

        public void Update()
        {
            if (m_frameTimer <= 0)
            {
                isActive = false;
            }
            else
            {
                m_frameTimer -= 1;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(m_texture, m_position, null, Color.White, 0, new Vector2(m_texture.Bounds.Center.X, m_texture.Bounds.Center.Y), 1, SpriteEffects.None, 1);
        }
    }
}
