using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    [Serializable]
    class GameValues
    {
        public int level, score, highScore;
        public float multiplier, initialTime;

        public GameValues ()
        {
            level = 0;
            highScore = 1000;
            score = 0;
            multiplier = 1;
            initialTime = 50;
        }

        public void Update()
        {
            multiplier = 1 + (level * 0.1f);
            if (level >= 10)
            {
                initialTime = 40;
            }
            if (level >= 20)
            {
                initialTime = 30;
            }
        }

#if DEBUG
        public void DebugDraw(SpriteBatch sb, SpriteFont font)
        {
            sb.DrawString(font, "Level: " + level, Vector2.Zero, Color.White);
            sb.DrawString(font, "Score: " + score, new Vector2(0, 24), Color.White);
        }
#endif
    }
}
