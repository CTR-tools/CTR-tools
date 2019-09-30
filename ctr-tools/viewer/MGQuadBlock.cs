using CTRFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace viewer
{
    class MGQuadBlock
    {
        public VertexPositionColor[] verts;
        public VertexPositionColor[] verts_wire;
        public VertexPositionColor[] verts_flag;

        public short[] indices;
        public short[] indices_flag;

        public static short[] indices_pattern = new short[] {
            0, 4, 5,
            5, 4, 6,
            4, 1, 6,
            6, 1, 7,
            5, 6, 2,
            2, 6, 8,
            6, 7, 8,
            8, 7, 3
        };

        public MGQuadBlock(Scene scn, int num, TerrainFlags qf, bool hide_invis)
        {
            List<VertexPositionColor> vts = new List<VertexPositionColor>();

            foreach (Vertex v in scn.vert)
                vts.Add(GetMonogameVertex(v, new Vector3(0, 0, 0)));

            verts = vts.ToArray();

            for (int i = 0; i < vts.Count; i++)
                vts[i] = new VertexPositionColor(vts[i].Position, Blend(vts[i].Color, Color.Blue));

            verts_flag = vts.ToArray();


            for (int i = 0; i < vts.Count; i++)
                vts[i] = new VertexPositionColor(vts[i].Position, Color.DarkRed);

            verts_wire = vts.ToArray();


            List<short> inds = new List<short>();
            List<short> indsf = new List<short>();

            foreach (QuadBlock qb in scn.quad)
            {
                //if (!qb.quadFlags.HasFlag(QuadFlags.InvisibleTriggers) | hide_invis)
                {
                    if (qb.terrainFlag.HasFlag(qf))
                    //if (qb.f2 > 0)
                    {
                        foreach (short s in indices_pattern)
                            indsf.Add(qb.ind[s]);
                    }
                    else
                    {
                        foreach (short s in indices_pattern)
                            inds.Add(qb.ind[s]);
                    }
                }
            }

            indices = inds.ToArray();
            indices_flag = indsf.ToArray();
            /*
            if (quadFlags.HasFlag(QuadFlags.Renderable))
            {
                vpc.Color = Blend(vpc.Color, Color.Green);


            */
        }

        public void Render(GraphicsDeviceManager graphics, BasicEffect effect)
        {
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                if (verts.Length > 0)
                    if (indices.Length > 0)
                        graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    verts, 0, verts.Length,
                    indices, 0, indices.Length / 3,
                    VertexPositionColor.VertexDeclaration
                   );

                if (verts_flag.Length > 0)
                    if (indices_flag.Length > 0)
                    {
                        graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            verts_flag, 0, verts_flag.Length,
                            indices_flag, 0, indices_flag.Length / 3,
                            VertexPositionColor.VertexDeclaration
                        );
                    }

            }
        }

        public void RenderWire(GraphicsDeviceManager graphics, BasicEffect effect)
        {
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                if (verts_wire.Length > 0)
                    if (indices.Length > 0)
                        graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    verts_wire, 0, verts.Length,
                    indices, 0, indices.Length / 3,
                    VertexPositionColor.VertexDeclaration
                   );
            }
        }

        public Color Blend(Color c1, Color c2)
        {
            Color x = Color.White;
            x.R = (byte)((c1.R + c2.R) / 2);
            x.G = (byte)((c1.G + c2.G) / 2);
            x.B = (byte)((c1.B + c2.B) / 2);
            return x;
        }

        public VertexPositionColor GetMonogameVertex(CTRFramework.Vertex v, Vector3 add_offset)
        {
            VertexPositionColor mono_v = new VertexPositionColor();
            mono_v.Position = new Microsoft.Xna.Framework.Vector3(v.coord.X, v.coord.Y, v.coord.Z) + add_offset;
            mono_v.Color = new Color(v.color.X, v.color.Y, v.color.Z);

            return mono_v;
        }

    }
}
