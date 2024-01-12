namespace Bone_King
{
    public enum ButtonEffect
    {
        Exit,
        Play,
        Instructions,
        Menu,
        Left,
        Right
    }

    public class ButtonManager
    {
        Game1 game;
        private ButtonManager(Game1 game)
        {
            this.game = game;
        }
        private static ButtonManager instance;
        public static ButtonManager Instance(Game1 game)
        {
            if (instance == null)
            {
                instance = new ButtonManager(game);
            }
            return instance;
        }

        public void PressButton(ButtonEffect effect)
        {
            switch (effect)
            {
                case ButtonEffect.Play:
                    game.gameState = Game1.GameState.Cutscene1;
                    break;
                case ButtonEffect.Instructions:
                    game.GoToInstructions();
                    break;
                case ButtonEffect.Exit:
                    game.Exit();
                    break;
                case ButtonEffect.Menu:
                    game.GoToMenu();
                    break;
                case ButtonEffect.Left:
                    game.InstructionsChangePage(-1);
                    break;
                case ButtonEffect.Right:
                    game.InstructionsChangePage(1);
                    break;
                default:
                    break;
            }
        }
    }
}