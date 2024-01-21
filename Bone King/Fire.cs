using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Bone_King
{
    class Fire : AnimatedSprite
    {
        public Rectangle collision;

        int spawnTimer;
        bool readyToSpawn;
        public bool spawning;

        const int SPAWNTIME = 120;
        public Fire (int x, int y, int width, int height, int animationSpeed, float layer):base(x, y, width, height, animationSpeed, layer)
        {
            collision = new Rectangle(x, y, width, height);
            spawnTimer = SPAWNTIME;
        }

        public void Reset()
        {
            spawnTimer = SPAWNTIME;
            readyToSpawn = false;
            spawning = false;
        }

        public void Update(List<SpecialBone> specialBones)
        {
            for (int i = 0; i < specialBones.Count; i++)
            {
                if (collision.Intersects(specialBones[i].collision) && specialBones[i].collision.Y > collision.Y)
                {
                    readyToSpawn = true;
                    specialBones.RemoveAt(i);
                }
            }

            if (readyToSpawn)
            {
                if (spawnTimer <= 0)
                {
                    spawnTimer = SPAWNTIME;
                    readyToSpawn = false;
                    spawning = true;
                }
                else
                {
                    spawnTimer -= 1;
                }
            }
        }
    }
}
