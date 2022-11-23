using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ctrviewer.Engine.Render
{
    //a collection of lines, used to render bsp tree
    public class LineCollection : List<VertexPositionColor>, IRenderable
    {
        public bool Sealed = false;

        private int numLines => verts is null ? 0 : verts.Length / 2;

        //vertex buffer used by monogame
        private VertexPositionColor[] verts;

        //essentially, opposite of Sealed state
        public bool CanWrite
        {
            get
            {
                if (!Sealed) return true;
                GameConsole.Write("Tried to add to a sealed line collection!");
                return false;
            }
        }

        public LineCollection()
        {
        }

        //adds single vert to the list, intended to be private
        private void PushVert(float x, float y, float z, Color color, float scale = 1f)
        {
            if (!CanWrite) return;

            var vert = new VertexPositionColor(scale != 1f ? new Vector3(x, y, z) * scale : new Vector3(x, y, z), color);
            Add(vert);
        }

        //adds single vert to the list, intended to be private
        private void PushVert(Vector3 pos, Color color, float scale = 1f)
        {
            if (!CanWrite) return;

            Add(new VertexPositionColor(scale != 1f ? pos * scale : pos, color));
        }

        //adds a line to the list
        public void PushLine(Vector3 from, Vector3 to, Color color, float scale = 1f)
        {
            if (!CanWrite) return;

            PushVert(from, color, scale);
            PushVert(to, color, scale);
        }

        //adds a box to the list
        public void PushBox(Vector3 min, Vector3 max, Color color, float scale = 1f)
        {
            if (!CanWrite) return;

            //prescale here to make less calculations later
            min = min * scale;
            max = max * scale;

            //8 vertices for the box
            var topA = new Vector3(min.X, max.Y, min.Z);
            var topB = new Vector3(min.X, max.Y, max.Z);
            var topC = max;
            var topD = new Vector3(max.X, max.Y, min.Z);

            var botA = min;
            var botB = new Vector3(min.X, min.Y, max.Z);
            var botC = new Vector3(max.X, min.Y, max.Z);
            var botD = new Vector3(max.X, min.Y, min.Z);

            //12 sides for the box
            PushLine(topA, botA, color);
            PushLine(topB, botB, color);
            PushLine(topC, botC, color);
            PushLine(topD, botD, color);

            PushLine(topA, topB, color);
            PushLine(topB, topC, color);
            PushLine(topC, topD, color);
            PushLine(topD, topA, color);

            PushLine(botA, botB, color);
            PushLine(botB, botC, color);
            PushLine(botC, botD, color);
            PushLine(botD, botA, color);
        }

        //dumps all vertices to buffer and locks the collection
        public void Seal()
        {
            verts = ToArray();
            Sealed = true;
        }

        //render stuff
        public void Draw(GraphicsDeviceManager graphics, BasicEffect effect, AlphaTestEffect alphaEffect = null)
        {
            if (!Sealed || graphics is null || effect is null) return;

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, verts, 0, numLines);
            }
        }
    }
}