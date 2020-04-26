using CTRFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using System.Collections;

namespace ctrviewer
{
    class MGQuadBlock
    {
        public TriList normal = new TriList();
        public TriList wire = new TriList();

        Dictionary<string, QuadList> normalq = new Dictionary<string, QuadList>();
        Dictionary<string, QuadList> animatedq = new Dictionary<string, QuadList>();
        QuadList wireq = new QuadList();

        public List<string> textureList
        {
            get
            {
                List<string> list = new List<string>();

                foreach(var n in normalq) 
                    if (!list.Contains(n.Key))
                        list.Add(n.Key);

                foreach (var n in animatedq) 
                    if (!list.Contains(n.Key))
                        list.Add(n.Key);

                return list;
            }
        }

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



        public MGQuadBlock(SkyBox sb)
        {
            normal.verts = new VertexPositionColorTexture[sb.cntVertex];
            normal.indices = new short[sb.faces.Count * 3];

            for (int i = 0; i < sb.cntVertex; i++)
            {
                normal.verts[i] = MGConverter.ToVptc(sb.verts[i], new CTRFramework.Shared.Vector2b(0, 0));
            }

            for (int i = 0; i < sb.faces.Count; i++)
            {
                normal.indices[i * 3 + 0] = sb.faces[i].X;
                normal.indices[i * 3 + 1] = sb.faces[i].Y;
                normal.indices[i * 3 + 2] = sb.faces[i].Z;
            }

            wire = new TriList(normal);

            for (int i = 0; i < wire.verts.Length; i++)
            {
                wire.verts[i].Color = Color.DarkRed;
            }
        }



        public MGQuadBlock(Scene s, Detail detail)
        {
            normal.textureName = "test";
            normal.textureEnabled = true;
            normal.verts = new VertexPositionColorTexture[s.quads.Count * 9];
            normal.indices = new short[s.quads.Count * 6 * 4];

            switch (detail)
            {
                case Detail.Low:
                    {
                        List<VertexPositionColorTexture> monolist = new List<VertexPositionColorTexture>();

                        for (int i = 0; i < s.quads.Count; i++)
                        {
                            if (!s.quads[i].quadFlags.HasFlag(QuadFlags.InvisibleTriggers))
                            {
                                List<CTRFramework.Vertex> vts = s.quads[i].GetVertexListq(s, -1);
                                monolist.Clear();

                                if (vts != null)
                                {
                                    foreach (Vertex cv in vts)
                                        monolist.Add(MGConverter.ToVptc(cv, cv.uv));

                                    wireq.PushQuad(monolist);


                                    TextureLayout t = s.quads[i].texlow;

                                    string texTag = t.Tag();

                                    if (normalq.ContainsKey(texTag))
                                    {
                                        normalq[texTag].PushQuad(monolist);
                                    }
                                    else
                                    {
                                        QuadList ql = new QuadList(monolist, true, texTag);
                                        normalq.Add(texTag, ql);
                                    }
                                }
                            }
                        }

                        foreach (var ql in normalq)
                            ql.Value.Seal();

                        wireq.Seal();
                        wireq.SetColor(Color.Black);

                        break;

                        /*
                        if (!s.quads[i].quadFlags.HasFlag(QuadFlags.InvisibleTriggers))
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                VertexPositionColorTexture v = new VertexPositionColorTexture();
                                CTRFramework.Vertex cv = s.verts[s.quads[i].ind[j]];
                                CTRFramework.TextureLayout tl = s.quads[i].texlow;

                                v.Position.X = cv.coord.X;
                                v.Position.Y = cv.coord.Y;
                                v.Position.Z = cv.coord.Z;

                                v.Color.A = 255;
                                v.Color.R = cv.color.X;
                                v.Color.G = cv.color.Y;
                                v.Color.B = cv.color.Z;

                                if (s.quads[i].ptrTexLow > 0)
                                {
                                    v.TextureCoordinate.X = tl.uv[j].X / 256.0f;
                                    v.TextureCoordinate.Y = tl.uv[j].Y / 256.0f;
                                }
                                else
                                {
                                    v.TextureCoordinate.X = 0;
                                    v.TextureCoordinate.Y = 0;
                                }

                                normal.verts[i * 4 + j] = v;
                            }

                            for (int k = 0; k < indices_pattern_low.Length; k++)
                            {
                                normal.indices[i * 6 + k] = (short)(i * 4 + indices_pattern_low[k]);
                            }

                        }*/

                    }

        

                case Detail.Med:
                    {
                        normal.verts = new VertexPositionColorTexture[s.quads.Count * 24];
                        normal.indices = new short[s.quads.Count * 24];

                        for (int i = 0; i < normal.indices.Length; i++)
                        {
                            normal.indices[i] = (short)i;
                        }

                        List<VertexPositionColorTexture> monolist = new List<VertexPositionColorTexture>();


                        for (int i = 0; i < s.quads.Count; i++)
                        {
                            if (!s.quads[i].quadFlags.HasFlag(QuadFlags.InvisibleTriggers))
                                for (int j = 0; j < 4; j++)
                                {

                                    List<CTRFramework.Vertex> vts = s.quads[i].GetVertexListq(s, j);
                                    monolist.Clear();

                                    if (vts != null)
                                    {
                                        foreach (Vertex cv in vts)
                                            monolist.Add(MGConverter.ToVptc(cv, cv.uv));

                                        wireq.PushQuad(monolist);

                                        CtrTex t = s.quads[i].tex[j];

                                        string texTag = t.midlods[2].Tag();

                                        if (!t.isAnimated)
                                        {
                                            if (normalq.ContainsKey(texTag))
                                            {
                                                normalq[texTag].PushQuad(monolist);
                                            }
                                            else
                                            {
                                                QuadList ql = new QuadList(monolist, true, texTag);
                                                normalq.Add(texTag, ql);
                                            }
                                        }
                                        else
                                        {
                                            if (animatedq.ContainsKey(texTag))
                                            {
                                                animatedq[texTag].PushQuad(monolist);
                                            }
                                            else
                                            {
                                                QuadList ql = new QuadList(monolist, true, texTag);
                                                ql.scrollingEnabled = true;
                                                animatedq.Add(texTag, ql);
                                            }
                                        }

                                    }
                                }
                        }

                        int numVerts = 0;
                        int numQuads = 0;
                        int numDrawCalls = 0;

                        foreach (var ql in normalq)
                        {
                            ql.Value.Seal();
                            numVerts += ql.Value.numVerts;
                            numQuads += ql.Value.numQuads;
                            numDrawCalls++;
                        }

                        foreach (var ql in animatedq)
                        {
                            ql.Value.Seal();
                            numVerts += ql.Value.numVerts;
                            numQuads += ql.Value.numQuads;
                            numDrawCalls++;
                        }

                        wireq.Seal();
                        wireq.SetColor(Color.Black);


                        Console.WriteLine("numVerts " + numVerts);
                        Console.WriteLine("numQuads " + numQuads);
                        Console.WriteLine("numDrawCalls " + numDrawCalls);

                        /*
                        //load trilist stuff
                        for (int i = 0; i < s.quads.Count; i++)
                        {
                            List<CTRFramework.Vertex> vts = s.quads[i].GetVertexList(s);
                            monolist = new List<VertexPositionColorTexture>();

                            foreach (Vertex cv in vts)
                            {
                                VertexPositionColorTexture v = new VertexPositionColorTexture();

                                v.Position.X = cv.coord.X;
                                v.Position.Y = cv.coord.Y;
                                v.Position.Z = cv.coord.Z;

                                v.Color.A = 255;
                                v.Color.R = cv.color.X;
                                v.Color.G = cv.color.Y;
                                v.Color.B = cv.color.Z;


                                if (s.quads[i].quadFlags.HasFlag(QuadFlags.Reverb))
                                {
                                    v.Color.R = 255;
                                    v.Color.G = 0;
                                    v.Color.B = 255;
                                }

                                v.TextureCoordinate.X = cv.uv.X;
                                v.TextureCoordinate.Y = cv.uv.Y;

                                monolist.Add(v);
                                
                            }

                            Array.Copy(
                                monolist.ToArray(), 0,
                                normal.verts, i * 24,
                                24);
                        }
                    */
                        break;
                    }
            }

            wire = new TriList(normal);

            for (int i = 0; i < wire.verts.Length; i++)
            {
                wire.verts[i].Color = Color.DarkRed;
            }
        }


        public void Update()
        {
            foreach (var ql in animatedq)
            {
                ql.Value.Update();
            }
        }

        public void Render(GraphicsDeviceManager graphics, BasicEffect effect)
        {
            /*
            if (normal.verts.Length > 0)
                if (normal.indices.Length > 0)
                    normal.Render(graphics, effect);
                    */

            Game1.clamp = true;
            Game1.UpdateSamplerState(graphics);

            foreach (var ql in normalq)
                ql.Value.Render(graphics, effect);
            

            Game1.clamp = false;
            Game1.UpdateSamplerState(graphics);

            foreach (var ql in animatedq)
                ql.Value.Render(graphics, effect);
        }

        public void RenderSky(GraphicsDeviceManager graphics, BasicEffect effect)
        {
            if (normal.verts.Length > 0)
                if (normal.indices.Length > 0)
                    normal.Render(graphics, effect);
        }

        public void RenderWire(GraphicsDeviceManager graphics, BasicEffect effect)
        {
            
            if (wire.verts.Length > 0)
                if (wire.indices.Length > 0)
                    wire.Render(graphics, effect);
                    
            wireq.Render(graphics, effect);
        }

        public Color Blend(Color c1, Color c2)
        {
            Color x = Color.White;
            x.R = (byte)((c1.R + c2.R) / 2);
            x.G = (byte)((c1.G + c2.G) / 2);
            x.B = (byte)((c1.B + c2.B) / 2);
            return x;
        }


    }
}