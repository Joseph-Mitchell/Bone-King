using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class Skull
    {
        enum AnimState
        {
            Ladder,
            MovingLeft,
            MovingRight,
            None
        }

        Texture2D m_spriteSheet;
        Vector2 m_position, m_velocity;
        Rectangle m_source;
        public Rectangle collision;

        AnimState m_animstate;

        int m_frameTimer;
        bool m_vulnerable, m_ladderIntersect;
        public bool isActive;

        const int ANIMATIONSPEED = 15;
        const float GRAVITY = 0.1f, MOVEMENTSPEED = 0.7f;

        public Skull (Texture2D spriteSheet, int x, int y)
        {
            m_spriteSheet = spriteSheet;
            m_position = new Vector2(x, y);
            m_source = new Rectangle(0, 0, 30, 32);
            collision = new Rectangle(x, y, 30, 32);

            m_animstate = AnimState.MovingRight;

            m_frameTimer = ANIMATIONSPEED;
            isActive = true;
        }

        public void Update(Background background, Random RNG, int maxX, Barry player)
        {
            //Gravity
            if (m_velocity.Y < GRAVITY * 30 && m_animstate != AnimState.Ladder)
            {
                m_velocity.Y += GRAVITY;
            }

            //Checks for collision with platforms and stops skull from falling through the ground
            for (int i = 0; i < background.platformHitBoxes.Length; i++)
            {
                if (background.platformHitBoxes[i].Intersects(collision))
                {
                    if (m_velocity.Y >= 0)
                    {
                        m_velocity.Y = 0;
                        m_position.Y = background.platformHitBoxes[i].Top - collision.Height + 1;
                    }
                }
            }

            //Chooses movement
            m_ladderIntersect = false;
            for (int i = 0; i < background.ladderHitBoxes.Length; i++)
            {
                if (i != 0 && i != 2 && i != 4 && i != 5 && i != 7 && i != 10)
                {
                    if (collision.Intersects(background.ladderHitBoxes[i]) && collision.X + (collision.Width / 2) > background.ladderHitBoxes[i].X + (background.ladderHitBoxes[i].Width / 3) && collision.X + (collision.Width / 2) < background.ladderHitBoxes[i].X + ((background.ladderHitBoxes[i].Width / 3) * 2) && m_animstate != AnimState.Ladder)
                    {
                        m_animstate = AnimState.Ladder;
                        m_position.X = background.ladderHitBoxes[i].X + (background.ladderHitBoxes[i].Width / 2) - (collision.Width / 2);
                    }
                }
                if (collision.Intersects(background.ladderHitBoxes[i]))
                {
                    m_ladderIntersect = true;
                }
            }

            //Moves depending on animstate
            if (m_animstate == AnimState.Ladder && !m_ladderIntersect)
            {
                m_animstate = AnimState.None;
            }
            if ((m_position.Y >= 359 || (m_position.Y >= 239 && m_position.Y < 299) || (m_position.Y >= 119 && m_position.Y < 179)) && m_animstate != AnimState.Ladder)
            {
                m_animstate = AnimState.MovingRight;
            }
            else if (((m_position.Y < 359 && m_position.Y >= 299) || (m_position.Y < 239 && m_position.Y >= 179) || (m_position.Y < 119)) && m_animstate != AnimState.Ladder)
            {
                m_animstate = AnimState.MovingLeft;
            }

            //Movement
            if (m_animstate == AnimState.MovingLeft)
            {
                m_velocity.X = -MOVEMENTSPEED;
            }
            else if (m_animstate == AnimState.MovingRight)
            {
                m_velocity.X = MOVEMENTSPEED;
            }
            else if (m_animstate == AnimState.Ladder)
            {
                m_velocity.X = 0;
                m_velocity.Y = -MOVEMENTSPEED;
            }
            else if (m_animstate == AnimState.None)
            {
                m_velocity.X = 0;
                m_velocity.Y = 0;
            }

            //Stops the skull from going off the screen
            if (collision.X + collision.Width > maxX)
            {
                m_position.X -= MOVEMENTSPEED;
            }
            if (collision.X < 0)
            {
                m_position.X += MOVEMENTSPEED;
            }

            //Changes the sprites for when Barry has the axe
            if (player.holdingAxe)
            {
                m_vulnerable = true;
            }
            else
            {
                m_vulnerable = false;
            }
            if (m_vulnerable)
            {
                m_source.Y = 32;
            }
            else
            {
                m_source.Y = 0;
            }

            collision.X = (int)m_position.X;
            collision.Y = (int)m_position.Y;

            m_position += m_velocity;
        }

        public void Draw(SpriteBatch sb, Game1 game)
        {
            if (game.paused == false)
            {
                if (m_frameTimer <= 0)
                {
                    m_source.X += m_source.Width;
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
            if (m_animstate == AnimState.MovingRight)
            {
                sb.Draw(m_spriteSheet, new Vector2((int)m_position.X, (int)m_position.Y), m_source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0.8f);
            }
            else
            {
                sb.Draw(m_spriteSheet, new Vector2((int)m_position.X, (int)m_position.Y), m_source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.8f);
            }
        }
#if DEBUG
        public void DebugDraw(SpriteBatch sb, Texture2D texture)
        {
            sb.Draw(texture, collision, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
        }
#endif
    }
}
