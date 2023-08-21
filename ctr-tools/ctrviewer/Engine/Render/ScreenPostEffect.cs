using Microsoft.Xna.Framework.Graphics;

namespace ctrviewer.Engine.Render
{
    public class ScreenPostEffect : BasicEffect
    {
        private bool mirrorHorizontally = false;
        public bool MirrorHorizontally
        {
            get => mirrorHorizontally;

            set
            {
                mirrorHorizontally = value;
                Parameters["SETTING_MIRROR_X"].SetValue(value);
            }
        }

        public ScreenPostEffect(GraphicsDevice device) : base(device)
        {
        }
    }
}