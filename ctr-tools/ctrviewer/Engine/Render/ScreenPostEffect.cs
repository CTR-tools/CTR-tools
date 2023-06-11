using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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