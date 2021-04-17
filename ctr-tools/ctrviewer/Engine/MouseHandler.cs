using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ctrviewer
{
    public class MouseHandler
    {
        public MouseState PreviousState;
        public MouseState CurrentState;

        public Point Resolution;
        public Vector2 VirtualPosition;

        public bool CaptureCursor = false;

        public void Init(int x, int y)
        {
            Resolution = new Point(x, y);
        }

        public void Update()
        {
            PreviousState = CurrentState;
            CurrentState = Mouse.GetState();

            if (CaptureCursor)
                if (CursorCaptured())
                    CurrentState = Mouse.GetState();

            VirtualPosition.X = CurrentState.X / Resolution.X;
            VirtualPosition.Y = CurrentState.Y / Resolution.Y;
        }

        private bool CursorCaptured()
        {
            bool result = false;

            //check X axis
            if (CurrentState.X <= 0)
            {
                Mouse.SetPosition(Resolution.X - 2, CurrentState.Y);
                result = true;
            }
            else if (CurrentState.X >= Resolution.X - 1)
            {
                Mouse.SetPosition(1, CurrentState.Y);
                result = true;
            }

            //check Y axis
            if (CurrentState.Y <= 0)
            {
                Mouse.SetPosition(CurrentState.X, Resolution.Y - 2);
                result = true;
            }
            else if (CurrentState.Y >= Resolution.Y - 1)
            {
                Mouse.SetPosition(CurrentState.X, 1);
                result = true;
            }

            return result;
        }
    }
}