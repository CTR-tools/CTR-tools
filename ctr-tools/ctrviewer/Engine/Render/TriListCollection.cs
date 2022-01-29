using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ctrviewer.Engine.Render
{
    public class TriListCollection : IRenderable
    {
        public List<TriList> Entries = new List<TriList>();

        public void Draw(GraphicsDeviceManager graphics, BasicEffect effect, AlphaTestEffect alpha)
        {
            foreach (var entry in Entries)
                entry.Draw(graphics, effect, alpha);
        }
    }
}