using Microsoft.Xna.Framework.Input;

namespace ctrviewer.Engine.Input
{
    public static class KeyboardHandler
    {
        private static KeyboardState oldState = Keyboard.GetState();
        private static KeyboardState newState = Keyboard.GetState();

        public static void Update()
        {
            oldState = newState;
            newState = Keyboard.GetState();
        }

        public static bool IsAltPressed => IsAnyDown(Keys.LeftAlt, Keys.RightAlt);
        public static bool IsShiftPressed => IsAnyDown(Keys.LeftShift, Keys.RightShift);
        public static bool IsControlPressed => IsAnyDown(Keys.LeftControl, Keys.RightControl);

        public static bool IsComboPressed(Keys helperkey, Keys key)
        {
            return IsDown(helperkey) && IsPressed(key);
        }

        public static bool IsDown(Keys key) => newState.IsKeyDown(key);

        public static bool IsAnyDown(params Keys[] keys)
        {
            foreach (var key in keys)
                if (IsDown(key))
                    return true;

            return false;
        }

        public static bool AreAllDown(params Keys[] keys)
        {
            foreach (var key in keys)
                if (!IsDown(key))
                    return false;

            return true;
        }

        public static bool IsPressed(Keys key) => newState.IsKeyDown(key) && !oldState.IsKeyDown(key);

        public static bool IsAnyPressed(params Keys[] keys)
        {
            foreach (var key in keys)
                if (IsPressed(key))
                    return true;

            return false;
        }
    }
}