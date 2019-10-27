using CTRFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace viewer
{
    class MGQuadBlock
    {
        public VertexPositionColorTexture[] verts;
        public VertexPositionColorTexture[] verts_wire;
        // public VertexPositionColorTexture[] verts_flag;

        public short[] indices;
        // public short[] indices_flag;

        public static short[] indices_pattern_low = new short[] { 0, 1, 2, 2, 1, 3 };

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


        public MGQuadBlock(Scene s, Detail detail)
        {
            verts = new VertexPositionColorTexture[s.quad.Count * 4];
            indices = new short[s.quad.Count * 6];

            switch (detail)
            {
                case Detail.Low:
                    {
                        for (int i = 0; i < s.quad.Count; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                VertexPositionColorTexture v = new VertexPositionColorTexture();
                                CTRFramework.Vertex cv = s.vert[s.quad[i].ind[j]];
                                CTRFramework.TextureLayout tl = s.quad[i].texlow;

                                v.Position.X = cv.coord.X;
                                v.Position.Y = cv.coord.Y;
                                v.Position.Z = cv.coord.Z;

                                v.Color.A = 255;
                                v.Color.R = cv.color.X;
                                v.Color.G = cv.color.Y;
                                v.Color.B = cv.color.Z;

                                if (s.quad[i].ptrTexLow > 0)
                                {
                                    v.TextureCoordinate.X = tl.uv[j].X / 256.0f;
                                    v.TextureCoordinate.Y = tl.uv[j].Y / 256.0f;
                                }
                                else
                                {
                                    v.TextureCoordinate.X = 0;
                                    v.TextureCoordinate.Y = 0;
                                }

                                verts[i * 4 + j] = v;
                            }

                            for (int k = 0; k < indices_pattern_low.Length; k++)
                            {
                                indices[i * 6 + k] = (short)(i * 4 + indices_pattern_low[k]);
                            }

                        }

                        break;
                    }

            }

            // verts_flag = verts;
            verts_wire = verts;
            // indices_flag = indices;
        }




        public MGQuadBlock(Scene scn, int num, TerrainFlags qf, bool hide_invis)
        {
            List<VertexPositionColorTexture> vts = new List<VertexPositionColorTexture>();

            foreach (Vertex v in scn.vert)
                vts.Add(GetMonogameVertex(v, new Vector3(0, 0, 0)));

            verts = vts.ToArray();

            for (int i = 0; i < vts.Count; i++)
                vts[i] = new VertexPositionColorTexture(vts[i].Position, Blend(vts[i].Color, Color.Blue), new Vector2(0, 0));

            //verts_flag = vts.ToArray();


            for (int i = 0; i < vts.Count; i++)
                vts[i] = new VertexPositionColorTexture(vts[i].Position, Color.DarkRed, new Vector2(0, 0));

            verts_wire = vts.ToArray();


            List<short> inds = new List<short>();
            List<short> indsf = new List<short>();


            VertexPositionColorTexture[] buf = vts.ToArray();

            foreach (QuadBlock qb in scn.quad)
            {
                //if (!qb.quadFlags.HasFlag(QuadFlags.InvisibleTriggers) | hide_invis)
                {
                    if (qb.terrainFlag.HasFlag(qf))
                    //if (qb.f2 > 0)
                    {

                        int i = 0;

                        foreach (short s in indices_pattern_low)
                        {
                            indsf.Add(qb.ind[s]);

                            try
                            {
                                buf[qb.ind[s]].TextureCoordinate = new Vector2(qb.texlow.uv[i].X, qb.texlow.uv[i].Y);
                            }
                            catch
                            {
                                buf[qb.ind[s]].TextureCoordinate = new Vector2(0, 0);
                            }

                            i++;
                        }
                    }
                    else
                    {
                        foreach (short s in indices_pattern_low)
                            inds.Add(qb.ind[s]);
                    }
                }
            }

            vts.Clear();
            vts.AddRange(buf);

            indices = inds.ToArray();
            //indices_flag = indsf.ToArray();
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
                    VertexPositionColorTexture.VertexDeclaration
                   );

                /*
                if (verts_flag.Length > 0)
                    if (indices_flag.Length > 0)
                    {
                        graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            verts_flag, 0, verts_flag.Length,
                            indices_flag, 0, indices_flag.Length / 3,
                            VertexPositionColorTexture.VertexDeclaration
                        );
                    }
                    */
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
                    VertexPositionColorTexture.VertexDeclaration
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

        public VertexPositionColorTexture GetMonogameVertex(CTRFramework.Vertex v, Vector3 add_offset)
        {
            VertexPositionColorTexture mono_v = new VertexPositionColorTexture();
            mono_v.Position = new Microsoft.Xna.Framework.Vector3(v.coord.X, v.coord.Y, v.coord.Z) + add_offset;
            mono_v.Color = new Color(v.color.X, v.color.Y, v.color.Z);
            mono_v.TextureCoordinate = new Vector2(0, 0);

            return mono_v;
        }

    }
}
