using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class InstructionsButton : Button
    {
        public InstructionsButton(Texture2D texture, int x, int y):base(texture, x, y)
        {
        }

        public void Pressed(Game1 game)
        {
            game.gameState = Game1.GameState.Instructions;
        }
    }
}
