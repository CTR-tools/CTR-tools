using Microsoft.Xna.Framework.Input;

namespace ctrviewer.Engine.Input
{
    public enum GameAction
    {
        MenuToggle,
        MenuConfirm,
        MenuBack,
        MenuUp,
        MenuDown,
        MenuLeft,
        MenuRight,
        MenuEnter,
        ForceQuit,
        ToggleConsole,
        ToggleFullscreen,
        Screenshot,
        FovUp,
        FovDown,
        KartAccelerate,
        KartBackPedal
    }


    public static class InputHandlers
    {
        public static void Update()
        {
            KeyboardHandler.Update();
            MouseHandler.Update();
            GamePadHandler.Update();
        }

        public static bool Process(GameAction action)
        {
            switch (action)
            {
                case GameAction.MenuToggle: return KeyboardHandler.IsAnyPressed(Keys.Escape) || GamePadHandler.IsPressed(SonyButtons.Start);

                case GameAction.MenuUp: return KeyboardHandler.IsAnyPressed(Keys.Up, Keys.W) || GamePadHandler.IsPressed(Buttons.DPadUp);
                case GameAction.MenuLeft: return KeyboardHandler.IsAnyPressed(Keys.Left, Keys.A) || GamePadHandler.IsPressed(Buttons.DPadLeft);
                case GameAction.MenuRight: return KeyboardHandler.IsAnyPressed(Keys.Right, Keys.D) || GamePadHandler.IsPressed(Buttons.DPadRight);
                case GameAction.MenuDown: return KeyboardHandler.IsAnyPressed(Keys.Down, Keys.S) || GamePadHandler.IsPressed(Buttons.DPadDown);

                case GameAction.MenuBack: return KeyboardHandler.IsAnyPressed(Keys.Back) || GamePadHandler.IsPressed(SonyButtons.Circle);
                case GameAction.MenuConfirm: return KeyboardHandler.IsAnyPressed(Keys.Enter) || GamePadHandler.IsPressed(SonyButtons.Cross);

                case GameAction.ForceQuit: return KeyboardHandler.IsAltComboPressed(Keys.F4) || GamePadHandler.AreAllDown(Buttons.Start, Buttons.Back);
                case GameAction.ToggleConsole: return KeyboardHandler.IsPressed(Keys.OemTilde) || (GamePadHandler.IsPressed(Buttons.Back) & !GamePadHandler.IsDown(Buttons.Start));

                case GameAction.ToggleFullscreen: return KeyboardHandler.IsAltComboPressed(Keys.Enter);

                case GameAction.Screenshot: return KeyboardHandler.IsPressed(Keys.PrintScreen);

                case GameAction.FovDown: return KeyboardHandler.IsDown(Keys.OemMinus);
                case GameAction.FovUp: return KeyboardHandler.IsDown(Keys.OemPlus);

                case GameAction.KartAccelerate: return KeyboardHandler.IsDown(Keys.W) || GamePadHandler.IsDown(SonyButtons.Cross);
                case GameAction.KartBackPedal: return KeyboardHandler.IsDown(Keys.S) || GamePadHandler.IsDown(SonyButtons.Square);
            }

            return false;
        }
    }
}