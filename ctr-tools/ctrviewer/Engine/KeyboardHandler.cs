using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace ctrviewer.Engine
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