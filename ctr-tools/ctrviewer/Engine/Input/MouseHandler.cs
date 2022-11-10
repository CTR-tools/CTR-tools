using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ctrviewer.Engine.Input
{
    public static class MouseHandler
    {
        private static MouseState oldState = Mouse.GetState();
        private static MouseState newState = Mouse.GetState();

        public static void Update()
        {
            oldState = newState;
            newState = Mouse.GetState();
        }
        public static void Reset()
        {
            newState = Mouse.GetState();
            oldState = newState;
        }

        public static bool IsLeftButtonHeld => newState.LeftButton == ButtonState.Pressed;
        public static bool IsRightButtonHeld => newState.RightButton == ButtonState.Pressed;
        public static bool IsLeftButtonPressed => newState.LeftButton == ButtonState.Pressed && oldState.LeftButton != ButtonState.Pressed;
        public static bool IsRightButtonPressed => newState.RightButton == ButtonState.Pressed && oldState.RightButton != ButtonState.Pressed;
        public static bool IsWheelPressed => newState.MiddleButton == ButtonState.Pressed;
        public static int X => newState.X;
        public static int Y => newState.Y;
        public static Point Position => new Point(X, Y);
        public static int DeltaX => newState.X - oldState.X;
        public static int DeltaY => newState.Y - oldState.Y;
        public static Point Delta => new Point(DeltaX, DeltaY);

        public static bool IsScrollingUp => newState.ScrollWheelValue > oldState.ScrollWheelValue;
        public static bool IsScrollingDown => newState.ScrollWheelValue < oldState.ScrollWheelValue;

        public static void Print()
        {
            GameConsole.Write($"Left:{IsLeftButtonPressed} Right:{IsRightButtonPressed} ScrollDown:{IsScrollingDown} ScrollUp:{IsScrollingUp} X:{X} Y:{Y} DeltaX:{DeltaX} DeltaY:{DeltaY}");
        }
    }
}