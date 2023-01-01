using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class Fire : AnimatedObject
    {
        public Rectangle collision;

        int m_spawnTimer;
        bool m_readyToSpawn;
        public bool spawning;

        const int SPAWNTIME = 120;
        public Fire (Texture2D spriteSheet, int x, int y, int width, int height, int animationSpeed, float layer):base(spriteSheet, x, y, width, height, animationSpeed, layer)
        {
            collision = new Rectangle(x, y, width, height);
            m_spawnTimer = SPAWNTIME;
        }

        public void Reset()
        {
            m_spawnTimer = SPAWNTIME;
            m_readyToSpawn = false;
            spawning = false;
        }

        public void Update(List<SpecialBone> specialBones)
        {
            for (int i = 0; i < specialBones.Count; i++)
            {
                if (collision.Intersects(specialBones[i].collision) && specialBones[i].collision.Y > collision.Y)
                {
                    m_readyToSpawn = true;
                    specialBones.RemoveAt(i);
                }
            }

            if (m_readyToSpawn)
            {
                if (m_spawnTimer <= 0)
                {
                    m_spawnTimer = SPAWNTIME;
                    m_readyToSpawn = false;
                    spawning = true;
                }
                else
                {
                    m_spawnTimer -= 1;
                }
            }
        }
    }
}
