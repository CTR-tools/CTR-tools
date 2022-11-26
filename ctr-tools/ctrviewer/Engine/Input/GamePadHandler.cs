using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ctrviewer.Engine.Input
{
    //maps playstation button names to xbox buttons
    public enum SonyButtons
    {
        Cross = Buttons.A,
        Circle = Buttons.B,
        Square = Buttons.X,
        Triangle = Buttons.Y,
        R1 = Buttons.RightShoulder,
        L1 = Buttons.LeftShoulder,
        R3 = Buttons.RightStick,
        L3 = Buttons.LeftStick,
        Start = Buttons.Start,
        Select = Buttons.Back
    }

    public static class GamePadHandler
    {
        public static PlayerIndex GamePadIndex = PlayerIndex.One;

        private static GamePadState oldState = GamePad.GetState(GamePadIndex);
        private static GamePadState newState = GamePad.GetState(GamePadIndex);

        public static float TriggerDeadZone = 0.1f;


        public static GamePadState State => newState;

        public static Vector2 RightStick => State.ThumbSticks.Right;
        public static Vector2 LeftStick => State.ThumbSticks.Left;

        public static float LeftTrigger => State.Triggers.Left;
        public static float RightTrigger => State.Triggers.Right;

        public static void Update()
        {
            if (!GamePad.GetState(GamePadIndex).IsConnected)
                DetectActiveGamePad();

            oldState = newState;
            newState = GamePad.GetState(GamePadIndex);
        }

        public static void Reset()
        {
            newState = GamePad.GetState(GamePadIndex);
            oldState = newState;
        }

        public static void DetectActiveGamePad()
        {
            for (var i = PlayerIndex.One; i <= PlayerIndex.Four; i++)
                if (GamePad.GetState(i).IsConnected)
                {
                    GamePadIndex = i;
                    break;
                }
        }

        public static bool IsPressed(Buttons button) => newState.IsButtonDown(button) && !oldState.IsButtonDown(button);
        public static bool IsDown(Buttons button) => newState.IsButtonDown(button);

        public static bool AreAllDown(params Buttons[] buttons)
        {
            foreach (var button in buttons)
                if (!IsDown(button))
                    return false;

            return true;
        }

        public static bool IsAnyDown(params Buttons[] buttons)
        {
            foreach (var button in buttons)
                if (IsDown(button))
                    return true;

            return false;
        }

        public static bool IsComboPressed(Buttons held, Buttons pressed) => IsDown(held) && IsPressed(pressed);
    }
}
