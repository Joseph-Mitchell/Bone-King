using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class BoneKing
    {
        enum AnimState
        {
            Standing,
            Throwing,
            BeatChest,
            SpecialBone
        }

        AnimState m_animState;

        Texture2D m_standing, m_throwing, m_beatChest, m_special;
        Vector2 m_position;
        Rectangle m_source;
        public Rectangle collision;

        float m_frameTimer;
        int m_angryCounter;
        bool m_wasAngry, m_firstBone;
        public bool boneDrop, specialBoneDrop;

        const int ANIMATIONSPEED = 30;

        public BoneKing(Texture2D standing, Texture2D throwing, Texture2D angery, Texture2D special, int x, int y)
        {
            m_animState = AnimState.Standing;

            m_standing = standing;
            m_throwing = throwing;
            m_beatChest = angery;
            m_special = special;
            m_position = new Vector2(x, y);
            m_source = new Rectangle(0, 0, 95, 77);
            collision = new Rectangle(x, y, 95, 70);

            m_frameTimer = ANIMATIONSPEED * 2;
        }

        public void Update(GameTime gt, Random RNG, GameValues level)
        {
            switch (m_animState)
            {
                case AnimState.Standing:
                    if (m_frameTimer <= 0)
                    {
                        int randomAngry = RNG.Next(0, 4);
                        if (randomAngry == 0 && m_wasAngry == false && level.level < 5)
                        {
                            m_animState = AnimState.BeatChest;
                            m_frameTimer = 30;
                        }
                        else
                        {
                            int randomSpecial = RNG.Next(0, 20);
                            if (randomSpecial == 0 || m_firstBone == false)
                            {
                                m_animState = AnimState.SpecialBone;
                                m_frameTimer = (ANIMATIONSPEED * 2) / level.multiplier;
                            }
                            else
                            {
                                m_animState = AnimState.Throwing;
                                m_frameTimer = (ANIMATIONSPEED * 2) / level.multiplier;
                                m_wasAngry = false;
                            }
                        }
                    }
                    else
                    {
                        m_frameTimer -= 1;
                    }
                    break;
                case AnimState.Throwing:
                    if (m_frameTimer <= 0)
                    {
                      m_source.X += m_source.Width;
                      if (m_source.X >= m_throwing.Width)
                      {
                            m_source.X = 0;
                            m_animState = AnimState.Standing;
                            boneDrop = true;
                      }
                      m_frameTimer = (ANIMATIONSPEED * 2) / level.multiplier;
                    }
                    else
                    {
                        m_frameTimer -= 1;
                    }
                    break;
                case AnimState.BeatChest:
                    if (m_frameTimer <= 0)
                    {
                        m_source.X += m_source.Width;
                        if (m_source.X >= m_beatChest.Width)
                        {
                            m_angryCounter += 1;
                            m_source.X = 0;
                        }
                        m_frameTimer = ANIMATIONSPEED / level.multiplier;
                        if (m_angryCounter == 2)
                        {
                            m_source.X = 0;
                            m_animState = AnimState.Standing;
                            m_frameTimer = (ANIMATIONSPEED * 2) / level.multiplier;
                            m_angryCounter = 0;
                            m_wasAngry = true;
                        }
                    }
                    else
                    {
                        m_frameTimer -= 1;
                    }
                    break;
                case AnimState.SpecialBone:
                    if (m_frameTimer <= 0)
                    {
                        m_source.X += m_source.Width;
                        if (m_source.X >= 184)
                        {
                            m_source.X = 0;
                            m_animState = AnimState.Standing;
                            specialBoneDrop = true;
                            m_firstBone = true;
                        }
                        m_frameTimer = (ANIMATIONSPEED * 2) / level.multiplier;
                    }
                    else
                    {
                        m_frameTimer -= 1;
                    }
                    break;
            }

            if (m_animState == AnimState.BeatChest)
            {
                m_source.Width = 92;
                m_source.Height = 70;
            }
            else
            {
                m_source.Width = 95;
                m_source.Height = 77;
            }
        }

        public void Reset(GameValues vals)
        {
            if (vals.level < 5)
            {
                m_animState = AnimState.Standing;
            }
            else
            {
                m_animState = AnimState.SpecialBone;
            }

            m_source = new Rectangle(0, 0, 95, 77);

            m_frameTimer = (ANIMATIONSPEED * 2) / vals.multiplier;

            m_firstBone = false;
        }

        public void Draw(SpriteBatch sb)
        {
            switch (m_animState)
            {
                case AnimState.Standing:
                    sb.Draw(m_standing, new Vector2((int)m_position.X, (int)m_position.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    break;
                case AnimState.Throwing:
                    sb.Draw(m_throwing, new Vector2((int)m_position.X, (int)m_position.Y), m_source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    break;
                case AnimState.BeatChest:
                    sb.Draw(m_beatChest, new Vector2((int)m_position.X, (int)m_position.Y), m_source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    break;
                case AnimState.SpecialBone:
                    sb.Draw(m_special, new Vector2((int)m_position.X, (int)m_position.Y), m_source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    break;
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
