using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    //This object is the background and collisions of the level itself
    class Level
    {
        Texture2D texture;
        public Rectangle[] platformHitBoxes, ladderHitBoxes, ladderTops;
        public Rectangle goal;
        public bool[] brokenLadder;

        public Level(Texture2D texture)
        {
            this.texture = texture;

            platformHitBoxes = new Rectangle[49];
            ladderHitBoxes = new Rectangle[13];
            ladderTops = new Rectangle[12];
            goal = new Rectangle(208, 0, 96, 63);
            brokenLadder = new bool[13];

            #region Platform Hitboxes
            //Bottom Layer
            platformHitBoxes[0] = new Rectangle(0, 415, 256, 3);
            platformHitBoxes[1] = new Rectangle(256, 413, 48, 3);
            platformHitBoxes[2] = new Rectangle(304, 411, 48, 3);
            platformHitBoxes[3] = new Rectangle(352, 409, 48, 3);
            platformHitBoxes[4] = new Rectangle(400, 407, 48, 3);
            platformHitBoxes[5] = new Rectangle(448, 405, 64, 3);

            //2nd Layer
            platformHitBoxes[6] = new Rectangle(400, 359, 48, 3);
            platformHitBoxes[7] = new Rectangle(352, 357, 48, 3);
            platformHitBoxes[8] = new Rectangle(304, 355, 48, 3);
            platformHitBoxes[9] = new Rectangle(256, 353, 48, 3);
            platformHitBoxes[10] = new Rectangle(208, 351, 48, 3);
            platformHitBoxes[11] = new Rectangle(160, 349, 48, 3);
            platformHitBoxes[12] = new Rectangle(112, 347, 48, 3);
            platformHitBoxes[13] = new Rectangle(64, 345, 48, 3);
            platformHitBoxes[14] = new Rectangle(0, 343, 64, 3);

            //3rd Layer
            platformHitBoxes[15] = new Rectangle(64, 299, 48, 3);
            platformHitBoxes[16] = new Rectangle(112, 297, 48, 3);
            platformHitBoxes[17] = new Rectangle(160, 295, 48, 3);
            platformHitBoxes[18] = new Rectangle(208, 293, 48, 3);
            platformHitBoxes[19] = new Rectangle(256, 291, 48, 3);
            platformHitBoxes[20] = new Rectangle(304, 289, 48, 3);
            platformHitBoxes[21] = new Rectangle(352, 287, 48, 3);
            platformHitBoxes[22] = new Rectangle(400, 285, 48, 3);
            platformHitBoxes[23] = new Rectangle(448, 283, 64, 3);

            //4th layer
            platformHitBoxes[24] = new Rectangle(400, 239, 48, 3);
            platformHitBoxes[25] = new Rectangle(352, 237, 48, 3);
            platformHitBoxes[26] = new Rectangle(304, 235, 48, 3);
            platformHitBoxes[27] = new Rectangle(256, 233, 48, 3);
            platformHitBoxes[28] = new Rectangle(208, 231, 48, 3);
            platformHitBoxes[29] = new Rectangle(160, 229, 48, 3);
            platformHitBoxes[30] = new Rectangle(112, 227, 48, 3);
            platformHitBoxes[31] = new Rectangle(64, 225, 48, 3);
            platformHitBoxes[32] = new Rectangle(0, 223, 64, 3);

            //5th layer
            platformHitBoxes[33] = new Rectangle(64, 179, 48, 3);
            platformHitBoxes[34] = new Rectangle(112, 177, 48, 3);
            platformHitBoxes[35] = new Rectangle(160, 175, 48, 3);
            platformHitBoxes[36] = new Rectangle(208, 173, 48, 3);
            platformHitBoxes[37] = new Rectangle(256, 171, 48, 3);
            platformHitBoxes[38] = new Rectangle(304, 169, 48, 3);
            platformHitBoxes[39] = new Rectangle(352, 167, 48, 3);
            platformHitBoxes[40] = new Rectangle(400, 165, 48, 3);
            platformHitBoxes[41] = new Rectangle(448, 163, 64, 3);

            //6th Layer
            platformHitBoxes[42] = new Rectangle(400, 119, 48, 3);
            platformHitBoxes[43] = new Rectangle(352, 117, 48, 3);
            platformHitBoxes[44] = new Rectangle(304, 115, 48, 3);
            platformHitBoxes[45] = new Rectangle(256, 113, 48, 3);
            platformHitBoxes[46] = new Rectangle(32, 111, 224, 3);

            //Top platforms
            platformHitBoxes[47] = new Rectangle(160, 79, 48, 3);
            platformHitBoxes[48] = new Rectangle(208, 63, 96, 3);
            #endregion

            #region Ladder Hitboxes
            ladderHitBoxes[0] = new Rectangle(210, 390, 16, 25);
            ladderHitBoxes[1] = new Rectangle(382, 360, 16, 49);
            ladderHitBoxes[2] = new Rectangle(238, 326, 16, 25);
            ladderHitBoxes[3] = new Rectangle(94, 302, 16, 43);
            ladderHitBoxes[4] = new Rectangle(172, 270, 16, 25);
            ladderHitBoxes[5] = new Rectangle(270, 266, 16, 25);
            ladderHitBoxes[6] = new Rectangle(382, 240, 16, 47);
            ladderHitBoxes[7] = new Rectangle(334, 210, 16, 25);
            ladderHitBoxes[8] = new Rectangle(188, 178, 16, 51);
            ladderHitBoxes[9] = new Rectangle(94, 182, 16, 43);
            ladderHitBoxes[10] = new Rectangle(228, 148, 16, 25);
            ladderHitBoxes[11] = new Rectangle(382, 120, 16, 47);
            ladderHitBoxes[12] = new Rectangle(288, 66, 16, 47);
            #endregion

            #region Ladder Tops
            ladderTops[0] = new Rectangle(210, 336, 16, 18);
            ladderTops[1] = new Rectangle(382, 342, 16, 18);
            ladderTops[2] = new Rectangle(238, 278, 16, 18);
            ladderTops[3] = new Rectangle(94, 284, 16, 18);
            ladderTops[4] = new Rectangle(172, 214, 16, 18);
            ladderTops[5] = new Rectangle(270, 218, 16, 18);
            ladderTops[6] = new Rectangle(382, 222, 16, 18);
            ladderTops[7] = new Rectangle(334, 154, 16, 18);
            ladderTops[8] = new Rectangle(188, 160, 16, 18);
            ladderTops[9] = new Rectangle(94, 164, 16, 18);
            ladderTops[10] = new Rectangle(228, 96, 16, 18);
            ladderTops[11] = new Rectangle(382, 102, 16, 18);
            #endregion

            for (int i = 0; i < ladderHitBoxes.Length; i++)
            {
                if(i == 0 || i == 2 || i == 4 || i == 5 || i == 7 || i == 10)
                {
                    brokenLadder[i] = true;
                }
                else
                {
                    brokenLadder[i] = false;
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }
#if DEBUG
        public void DebugDraw(SpriteBatch sb, Texture2D hitBoxTexture)
        {
            for (int i = 0; i < platformHitBoxes.Length; i++)
            {
                sb.Draw(hitBoxTexture, platformHitBoxes[i], Color.Red);
            }
            for (int i = 0; i < ladderHitBoxes.Length; i++)
            {
                sb.Draw(hitBoxTexture, ladderHitBoxes[i], Color.Green);
            }
            for (int i = 0; i < ladderTops.Length; i++)
            {
                sb.Draw(hitBoxTexture, ladderTops[i], null, Color.Blue, 0, Vector2.Zero, SpriteEffects.None, 1);
            }
            sb.Draw(hitBoxTexture, goal, null, Color.Yellow, 0, Vector2.Zero, SpriteEffects.None, 0.1f);
        }
#endif
    }
}
