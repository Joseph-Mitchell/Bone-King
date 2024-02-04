using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Bone_King
{
    class Fire
    {
        public Rectangle collision;

        AnimatedSprite sprite;

        int spawnTimer;
        bool readyToSpawn;
        public bool spawning;

        const int SPAWNTIME = 120;
        public Fire (int x, int y, int width, int height, int animationSpeed, float layer)
        {
            sprite = new AnimatedSprite(x, y, animationSpeed, layer, new Vector2(30, 36)); 

            collision = new Rectangle(x, y, width, height);
            spawnTimer = SPAWNTIME;
        }

        public void Load(Texture2D spriteSheet)
        {
            sprite.Load(spriteSheet);
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

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }
    }
}
