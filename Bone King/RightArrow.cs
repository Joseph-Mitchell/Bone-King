using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class RightArrow : Button
    {
        public RightArrow(Texture2D texture, int x, int y):base(texture, x, y)
        { }

        public void Pressed(Game1 game)
        {
            game.instructionPage += 1;
        }
    }
}
