using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ctrviewer.Engine.Input
{
    public static class MouseHandler
    {
        #region [Mouse state handling]

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

        #endregion

        #region [Mouse position handling]
        public static int X => newState.X;
        public static int Y => newState.Y;

        private static Point PositionPoint = new Point(0, 0);
        public static Point Position
        {
            get
            {
                PositionPoint.X = X;
                PositionPoint.Y = Y;
                return PositionPoint;
            }
        }

        public static int DeltaX => newState.X - oldState.X;
        public static int DeltaY => newState.Y - oldState.Y;

        private static Point DeltaPoint = new Point(0, 0);

        public static Point Delta
        {
            get
            {
                DeltaPoint.X = DeltaX;
                DeltaPoint.Y = DeltaY;
                return DeltaPoint;
            }
        }
        #endregion

        //held refers to down
        public static bool IsLeftButtonHeld => newState.LeftButton == ButtonState.Pressed;
        public static bool IsRightButtonHeld => newState.RightButton == ButtonState.Pressed;
        public static bool IsWheelHeld => newState.MiddleButton == ButtonState.Pressed;

        //pressed refers to just changed state to down
        public static bool IsLeftButtonPressed => newState.LeftButton == ButtonState.Pressed && oldState.LeftButton != ButtonState.Pressed;
        public static bool IsRightButtonPressed => newState.RightButton == ButtonState.Pressed && oldState.RightButton != ButtonState.Pressed;
        public static bool IsWheelPressed => newState.MiddleButton == ButtonState.Pressed && oldState.MiddleButton != ButtonState.Pressed;

        public static bool IsScrollingUp => newState.ScrollWheelValue > oldState.ScrollWheelValue;
        public static bool IsScrollingDown => newState.ScrollWheelValue < oldState.ScrollWheelValue;

        public static bool Moved => (newState.X != oldState.X) || (newState.Y != oldState.Y);

        public static void Print()
        {
            GameConsole.Write($"Left:{IsLeftButtonPressed} Right:{IsRightButtonPressed} ScrollDown:{IsScrollingDown} ScrollUp:{IsScrollingUp} X:{X} Y:{Y} DeltaX:{DeltaX} DeltaY:{DeltaY}");
        }
    }
}