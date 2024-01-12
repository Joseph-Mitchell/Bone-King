using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Bone_King
{
    class ParticleBurst
    {
        private Random random;
        public Vector2 emitter;
        private List<Particle> particles;
        private List<Texture2D> textures;

        private int burstAmount;
        public int burstDelay, initialDelay;
        private float maxXV, minXV;

        public ParticleBurst(List<Texture2D> Textures, float x, float y, int amount, int delay, float maxXveclocity, float minXvelocity)
        {
            emitter = new Vector2(x, y);
            textures = Textures;
            particles = new List<Particle>();
            random = new Random();
            burstAmount = amount;
            burstDelay = 0;
            initialDelay = delay;
            maxXV = maxXveclocity;
            minXV = minXvelocity;
        }

        private void GenerateNewParticle()
        {
            Texture2D texture = textures[random.Next(textures.Count)];
            Vector2 position = emitter;
            Vector2 velocity = new Vector2(maxXV * (float)(random.NextDouble()) + minXV, 3f * (float)(random.NextDouble() - 2f));
            float rotation = 0;
            float rotationSpeed = 0.1f * (float)(random.NextDouble() * 2 - 1);
            Color color = new Color((float)random.Next(0, 2), (float)random.Next(0, 2), (float)random.Next(0, 2));
            while (color == new Color(0, 0, 0))
            {
                color = new Color((float)random.Next(0, 2), (float)random.Next(0, 2), (float)random.Next(0, 2));
            }
            float size = (float)random.NextDouble();
            int lifeTime = 100 + random.Next(40);

            particles.Add(new Particle(texture, position.X, position.Y, velocity.X, velocity.Y, color, rotation, rotationSpeed, size, lifeTime));
        }

        public void Update()
        {
            if (burstDelay <= 0)
            {
                for (int i = 0; i < burstAmount; i++)
                {
                    GenerateNewParticle();
                }

                burstDelay = initialDelay;
            }
            else
            {
                burstDelay -= 1;
            }

            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Update();

                if (particles[i].lifeTime <= 0)
                {
                    particles.RemoveAt(i);
                    i--;
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Draw(sb);
            }
        }
    }
}
