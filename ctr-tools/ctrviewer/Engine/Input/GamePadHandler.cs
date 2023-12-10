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
        public delegate void NoArgsEvent();

        public static NoArgsEvent onGamepadConnected;
        public static NoArgsEvent onGamepadDisconnected;

        private static bool gamePadConnected = false;
        public static bool GamePadConnected {
            get { return gamePadConnected; }
            set
            {
                //if value changed
                if (gamePadConnected != value)
                {
                    //trigger event
                    if (value)
                    {
                        GameConsole.Write("gamepad connected!");
                        onGamepadConnected?.Invoke();
                    }
                    else
                    {
                        GameConsole.Write("gamepad disconnected!");
                        onGamepadDisconnected?.Invoke();
                    }

                    //update value
                    gamePadConnected = value;
                }
            }
        }

        public static PlayerIndex GamePadIndex = PlayerIndex.One;

        private static GamePadState oldState = GamePad.GetState(GamePadIndex);
        private static GamePadState newState = GamePad.GetState(GamePadIndex);

        public static float TriggerDeadZone = 0f;

        public static GamePadState State => newState;

        public static Vector2 RightStick => CheckDeadZone(State.ThumbSticks.Right);
        public static Vector2 LeftStick => CheckDeadZone(State.ThumbSticks.Left);

        private static Vector2 CheckDeadZone(Vector2 value)
        {
            //implement some range mapping, check UV mapping code
            //currently it just chops off small values

            //process X axis
            if (value.X > 0)
            {
                if (value.X < TriggerDeadZone) value.X = 0;
            }
            else if (value.X < 0)
            { 
                if (value.X > -TriggerDeadZone) value.X = 0;
            }

            //process Y axis
            if (value.Y > 0)
            {
                if (value.Y < TriggerDeadZone) value.Y = 0;
            }
            else if (value.Y < 0)
            {
                if (value.Y > -TriggerDeadZone) value.Y = 0;
            }

            return value;
        }

        public static float LeftTrigger => State.Triggers.Left;
        public static float RightTrigger => State.Triggers.Right;

        public static void Update()
        {
            // check if we still have the controller
            if (!GamePad.GetState(GamePadIndex).IsConnected)
            {
                GamePadConnected = false;
            }

            if (!GamePadConnected)
                DetectActiveGamePad();

            if (GamePadConnected)
            {
                oldState = newState;
                newState = GamePad.GetState(GamePadIndex);
            }
        }

        public static void Reset()
        {
            if (!GamePadConnected)
            {
                DetectActiveGamePad();
            }

            if (GamePadConnected)
            {
                newState = GamePad.GetState(GamePadIndex);
                oldState = newState;
            }
        }

        public static void DetectActiveGamePad()
        {
            //poll every xinput controller slot

            for (var i = PlayerIndex.One; i <= PlayerIndex.Four; i++)
                if (GamePad.GetState(i).IsConnected)
                {
                    GamePadConnected = true;
                    GamePadIndex = i;

                    Reset();

                    return;
                }

            //give up
            GamePadConnected = false;
        }

        public static bool IsPressed(Buttons button) => newState.IsButtonDown(button) && !oldState.IsButtonDown(button);
        public static bool IsPressed(SonyButtons button) => IsPressed((Buttons)button);
        public static bool IsDown(Buttons button) => newState.IsButtonDown(button);
        public static bool IsDown(SonyButtons button) => IsDown((Buttons)button);
        public static bool AreAllDown(params Buttons[] buttons)
        {
            foreach (var button in buttons)
                if (!IsDown(button))
                    return false;

            return true;
        }

        /// <summary>
        /// Detects whether any button from the list is pressed.
        /// </summary>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public static bool IsAnyDown(params Buttons[] buttons)
        {
            foreach (var button in buttons)
                if (IsDown(button))
                    return true;

            return false;
        }

        /// <summary>
        /// Detect whether one button is pressed while another is held.
        /// </summary>
        /// <param name="held"></param>
        /// <param name="pressed"></param>
        /// <returns></returns>
        public static bool IsComboPressed(Buttons held, Buttons pressed) => IsDown(held) && IsPressed(pressed);
    }
}