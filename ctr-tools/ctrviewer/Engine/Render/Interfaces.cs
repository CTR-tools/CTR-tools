using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ctrviewer.Engine.Render
{
    public interface IRenderable
    {
        public void Draw(GraphicsDeviceManager graphics, BasicEffect effect, AlphaTestEffect alpha);
    }
}