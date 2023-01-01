using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class SpecialBone
    {

        Texture2D m_spriteSheet;
        Vector2 m_position, m_velocity;
        Rectangle m_source;
        public Rectangle collision;

        int m_frameTimer;
        bool m_collisionCheck, m_collisionCheckOld;
        public bool isActive;

        const int ANIMATIONSPEED = 4;
        const float GRAVITY = 0.1f;

        public SpecialBone(Texture2D spritesheet, int x, int y)
        {
            m_spriteSheet = spritesheet;
            m_position = new Vector2(x, y);
            m_velocity = Vector2.Zero;

            m_source = new Rectangle(0, 0, 50, 33);

            collision = new Rectangle(x + 4, y + 9, 42, 15);

            m_frameTimer = ANIMATIONSPEED;
            isActive = true;
        }

        public void Update(GameTime gt, Background background, Game1 game)
        {
            m_position += m_velocity;
            m_collisionCheck = false;

            //Gravity
            if (m_velocity.Y < GRAVITY * 30)
            {
                m_velocity.Y += GRAVITY;
            }

            //Checks if the bone hits a platform
            for (int i = 0; i < background.platformHitBoxes.Length; i++)
            {
                if (collision.Intersects(background.platformHitBoxes[i]))
                {
                    m_collisionCheck = true;
                }
            }

            //Slows down the bone every time it hits a platform
            if (m_collisionCheck && m_collisionCheckOld == false)
            {
                m_velocity.Y = 0;
                game.bang.Play();
            }

            //Makes collision rectangles follow the main sprite position
            collision = new Rectangle((int)m_position.X + 4, (int)m_position.Y + 9, 42, 15);

            m_collisionCheckOld = m_collisionCheck;

            if (m_frameTimer <= 0)
            {
                m_source.X = (m_source.X + m_source.Width);
                if (m_source.X >= m_spriteSheet.Width)
                {
                    m_source.X = 0;
                }
                m_frameTimer = ANIMATIONSPEED;
            }
            else
            {
                m_frameTimer -= 1;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(m_spriteSheet, new Vector2((int)m_position.X, (int)m_position.Y), m_source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.91f);
        }

#if DEBUG
        public void DebugDraw(SpriteBatch sb, Texture2D hitBoxTexture, SpriteFont debugFont)
        {
            sb.Draw(hitBoxTexture, collision, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
        }
#endif
    }
}
