using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bone_King
{
    /*This class controls all input. It allows using either a controler or a keyboard, and minimalizes complexity in other areas
    of the game*/
    class Input
    {
        KeyboardState keyboard;
        GamePadState pad;

        public bool up, down, left, right, action, pause;
#if DEBUG
        public bool debug;
#endif

        public void Update()
        {
            keyboard = Keyboard.GetState();
            pad = GamePad.GetState(PlayerIndex.One);

            if (keyboard.IsKeyDown(Keys.Up) || pad.DPad.Up == ButtonState.Pressed)
            {
                up = true;
            }
            else
            {
                up = false;
            }
            if (keyboard.IsKeyDown(Keys.Down) || pad.DPad.Down == ButtonState.Pressed)
            {
                down = true;
            }
            else
            {
                down = false;
            }
            if (keyboard.IsKeyDown(Keys.Right) || pad.DPad.Right == ButtonState.Pressed)
            {
                right = true;
            }
            else
            {
                right = false;
            }
            if (keyboard.IsKeyDown(Keys.Left) || pad.DPad.Left == ButtonState.Pressed)
            {
                left = true;
            }
            else
            {
                left = false;
            }
            if (keyboard.IsKeyDown(Keys.Space) || pad.Buttons.A == ButtonState.Pressed)
            {
                action = true;
            }
            else
            {
                action = false;
            }
            if (keyboard.IsKeyDown(Keys.Escape) || pad.Buttons.Start == ButtonState.Pressed)
            {
                pause = true;
            }
            else
            {
                pause = false;
            }
#if DEBUG
            if (keyboard.IsKeyDown(Keys.OemTilde))
            {
                debug = true;
            }
            else
            {
                debug = false;
            }
#endif
        }
    }
}
