using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class Bone
    {
        public enum State
        {
            None,
            RollingRight,
            RollingLeft,
            FallingRight,
            FallingLeft,
            Ladder
        }

        Texture2D m_normalSprites, m_ladderSprites;
        Vector2 m_position, m_velocity;
        Rectangle m_source;
        public Rectangle groundCollision, playerCollision, scoreRectangle;

        public State animState;

        float m_frameTimer;
        bool m_collisionCheck, m_ladderCheck;
        public bool isActive;

        const int ANIMATIONSPEED = 4;
        const float GRAVITY = 0.1f, BONESPEED = 1.5f;

        public Bone(Texture2D normal, Texture2D ladder, GameValues level, int x, int y)
        {
            m_normalSprites = normal;
            m_ladderSprites = ladder;
            m_position = new Vector2(x, y);
            m_velocity = Vector2.Zero;

            m_source = new Rectangle(0, 0, 33, 33);

            groundCollision = new Rectangle(x - 7, y - 7, 15, 15);
            playerCollision = new Rectangle(x - 7, y - 7, 15, 15);
            scoreRectangle = new Rectangle(x - 2, y - 17, 4, 10);

            animState = State.RollingRight;

            m_frameTimer = ANIMATIONSPEED;
            isActive = true;
        }

        public void Update(GameTime gt, Background background, Random RNG, GameValues level)
        {
            m_position += m_velocity;
            m_collisionCheck = false;

            //Checks if the bone is on any of the platforms or ladders
            for (int i = 0; i < background.ladderTops.Length; i++)
            {
                if (groundCollision.Intersects(background.ladderTops[i]) && groundCollision.X + (groundCollision.Width / 2) > background.ladderTops[i].X + (background.ladderTops[i].Width / 3) && groundCollision.X + (groundCollision.Width / 2) < background.ladderTops[i].X + ((background.ladderTops[i].Width / 3) * 2) && m_ladderCheck == false)
                {
                    int potato = RNG.Next(0, 8);
                    if (potato == 0)
                    {
                        m_ladderCheck = true;
                        m_position.X = background.ladderTops[i].X + (background.ladderTops[i].Width / 2);
                        m_position.Y += 10;
                    }
                }
            }
            for (int i = 0; i < background.platformHitBoxes.Length; i++)
            {
                if (groundCollision.Intersects(background.platformHitBoxes[i]) && m_ladderCheck == false)
                {
                    m_collisionCheck = true;
                    m_position.Y = background.platformHitBoxes[i].Y - 7;
                }
            }

            //Checks if the bone is at the bottom of a ladder
            if (m_ladderCheck)
            {
                m_ladderCheck = false;
                for (int i = 0; i < background.ladderTops.Length; i++)
                {
                    if (groundCollision.Intersects(background.ladderTops[i]))
                    {
                        m_ladderCheck = true;
                    }
                }
            }

            //Changes state depending on bones position and collisions
            if (m_collisionCheck && m_ladderCheck == false && animState != State.Ladder)
            {
                if ((m_position.Y >= 155 && m_position.Y <= 213) || (m_position.Y >= 275 && m_position.Y <= 329) || m_position.Y >= 397)
                {
                    animState = State.RollingLeft;
                }
                else
                {
                    animState = State.RollingRight;
                }
            }
            else if (m_ladderCheck == false && animState != State.Ladder)
            {
                if ((m_position.Y >= 155 && m_position.Y <= 213) || (m_position.Y >= 275 && m_position.Y <= 329) || m_position.Y >= 397)
                {
                    animState = State.FallingLeft;
                }
                else if (m_velocity.X > 0)
                {
                    animState = State.FallingRight;
                }
            }
            else
            {
                animState = State.Ladder;
            }

            //Times the animation
            if (m_frameTimer <= 0)
            {
                m_source.X = (m_source.X + m_source.Width);

                if (animState == State.Ladder)
                {
                    if (m_source.X >= m_ladderSprites.Width)
                    {
                        m_source.X = 0;
                    }
                }
                else
                {
                    if (m_source.X >= m_normalSprites.Width)
                    {
                        m_source.X = 0;
                    }
                }

                m_frameTimer = ANIMATIONSPEED / level.multiplier;
            }
            else
            {
                m_frameTimer -= 1;
            }

            //Changes values depending on state
            switch (animState)
            {
                case State.RollingLeft:
                    m_velocity.X = -(BONESPEED * level.multiplier);
                    m_velocity.Y = 0;
                    m_source.Width = 33;
                    m_source.Height = 33;
                    if (m_source.X % 33 != 0)
                    {
                        m_source.X = 0;
                    }
                    groundCollision.X = (int)m_position.X - 7;
                    groundCollision.Y = (int)m_position.Y - 7;
                    playerCollision = new Rectangle((int)m_position.X - 7, (int)m_position.Y - 5, 15, 13);
                    scoreRectangle = new Rectangle((int)m_position.X - 2, (int)m_position.Y - 27, 4, 20);
                    break;
                case State.RollingRight:
                    m_velocity.X = BONESPEED * level.multiplier;
                    m_velocity.Y = 0;
                    m_source.Width = 33;
                    m_source.Height = 33;
                    if (m_source.X % 33 != 0)
                    {
                        m_source.X = 0;
                    }
                    groundCollision.X = (int)m_position.X - 7;
                    groundCollision.Y = (int)m_position.Y - 7;
                    playerCollision = new Rectangle((int)m_position.X - 7, (int)m_position.Y - 5, 15, 13);
                    scoreRectangle = new Rectangle((int)m_position.X - 2, (int)m_position.Y - 27, 4, 20);
                    break;
                case State.FallingLeft:
                    if (m_position.X + (groundCollision.Width / 2) < 64)
                    {
                        m_velocity.X = -(BONESPEED * level.multiplier) / 2;
                    }
                    else
                    {
                        m_velocity.X = -(BONESPEED * level.multiplier);
                    }
                    m_velocity.Y += GRAVITY * level.multiplier;
                    m_source.Width = 33;
                    m_source.Height = 33;
                    if (m_source.X % 33 != 0)
                    {
                        m_source.X = 0;
                    }
                    groundCollision.X = (int)m_position.X - 7;
                    groundCollision.Y = (int)m_position.Y - 7;
                    playerCollision = new Rectangle((int)m_position.X - 7, (int)m_position.Y - 5, 15, 13);
                    scoreRectangle = new Rectangle((int)m_position.X - 2, (int)m_position.Y - 27, 4, 20);
                    break;
                case State.FallingRight:
                    if (m_position.X - (groundCollision.Width / 2) > 448)
                    {
                        m_velocity.X = (BONESPEED * level.multiplier) / 2;
                    }
                    else
                    {
                        m_velocity.X = BONESPEED * level.multiplier;
                    }
                    m_velocity.Y += GRAVITY * level.multiplier;
                    m_source.Width = 33;
                    m_source.Height = 33;
                    if (m_source.X % 33 != 0)
                    {
                        m_source.X = 0;
                    }
                    groundCollision.X = (int)m_position.X - 7;
                    groundCollision.Y = (int)m_position.Y - 7;
                    playerCollision = new Rectangle((int)m_position.X - 7, (int)m_position.Y - 5, 15, 13);
                    scoreRectangle = new Rectangle((int)m_position.X - 2, (int)m_position.Y - 27, 4, 20);
                    break;
                case State.Ladder:
                    m_velocity.X = 0;
                    m_velocity.Y = (BONESPEED * level.multiplier) * 0.6f;
                    if (m_collisionCheck)
                    {
                        animState = State.None;
                    }
                    m_source.Width = 50;
                    m_source.Height = 33;
                    if (m_source.X % 50 != 0)
                    {
                        m_source.X = 0;
                    }
                    groundCollision.X = (int)m_position.X - 7;
                    groundCollision.Y = (int)m_position.Y - 7;
                    playerCollision = new Rectangle((int)m_position.X - 20, (int)m_position.Y - 7, 40, 15);
                    scoreRectangle = new Rectangle((int)m_position.X - 2, (int)m_position.Y - 27, 4, 20);
                    break;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            //Chooses which spritesheet to use, and whether to flip it
            if (animState == State.FallingLeft || animState == State.RollingLeft)
            {
                sb.Draw(m_normalSprites, new Vector2((int)m_position.X, (int)m_position.Y), m_source, Color.White, 0, new Vector2(m_source.Width / 2, m_source.Height / 2), 1, SpriteEffects.FlipHorizontally, 0.9f);
            }
            else if (animState == State.FallingRight || animState == State.RollingRight)
            {
                sb.Draw(m_normalSprites, new Vector2((int)m_position.X, (int)m_position.Y), m_source, Color.White, 0, new Vector2(m_source.Width / 2, m_source.Height / 2), 1, SpriteEffects.None, 0.9f);
            }
            else if (animState == State.Ladder)
            {
                sb.Draw(m_ladderSprites, new Vector2((int)m_position.X, (int)m_position.Y), m_source, Color.White, 0, new Vector2(m_source.Width / 2, m_source.Height / 2), 1, SpriteEffects.None, 0.9f);
            }
        }
#if DEBUG
        public void DebugDraw(SpriteBatch sb, Texture2D hitBoxTexture, SpriteFont debugFont)
        {
            sb.Draw(hitBoxTexture, groundCollision, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            sb.Draw(hitBoxTexture, playerCollision, null, Color.Blue, 0, Vector2.Zero, SpriteEffects.None, 0.99f);
            sb.Draw(hitBoxTexture, scoreRectangle, null, Color.Yellow, 0, Vector2.Zero, SpriteEffects.None, 1);
        }
#endif
    }
}
