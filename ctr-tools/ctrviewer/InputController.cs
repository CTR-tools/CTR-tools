using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ctrviewer
{
    public enum InputType
    {
        Keyboard,
        GamePad
    }

    class InputController : GameComponent
    {
        private InputType inputType = InputType.GamePad;
        public InputType InputType => inputType;

        PlayerIndex activeGamePad = PlayerIndex.One;

        GamePadState oldstate;
        GamePadState newstate;

        KeyboardState oldkb;
        KeyboardState newkb;

        public InputController(Game game) : base(game)
        {
        }

        public void Update(GameTime gametime)
        {
            switch (inputType)
            {
                case InputType.GamePad: UpdateGamePadState(); break;
                case InputType.Keyboard: UpdateKeyboardState(); break;
            }
        }

        public void SetActiveGamePad(PlayerIndex pi)
        {
            activeGamePad = pi;
        }

        public void CheckGamePad(PlayerIndex pi)
        {
            if (!newstate.IsConnected)
                for (PlayerIndex i = PlayerIndex.One; i <= PlayerIndex.Four; i++)
                {
                    GamePadState state = GamePad.GetState(i);
                    if (state.IsConnected)
                    {
                        SetActiveGamePad(i);
                        return;
                    }
                }
        }

        public void UpdateGamePadState()
        {
            oldstate = newstate;
            newstate = GamePad.GetState(activeGamePad);
        }

        public void UpdateKeyboardState()
        {
            oldkb = newkb;
            newkb = new KeyboardState();
        }
    }
}