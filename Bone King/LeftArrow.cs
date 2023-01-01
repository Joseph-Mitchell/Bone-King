using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class LeftArrow : Button
    {
        public LeftArrow(Texture2D texture, int x, int y) : base(texture, x, y)
        { }

        public void Pressed(Game1 game)
        {
            game.instructionPage -= 1;
        }
    }
}