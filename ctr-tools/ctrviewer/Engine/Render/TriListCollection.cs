using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ctrviewer.Engine.Render
{
    public class TriListCollection : List<TriList>, IRenderable
    {
        public void Draw(GraphicsDeviceManager graphics, BasicEffect effect, AlphaTestEffect alpha)
        {
            foreach (var entry in this)
                entry.Draw(graphics, effect, alpha);
        }
    }
}