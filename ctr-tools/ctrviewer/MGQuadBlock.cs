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
        public static Color ToD = new Color(1f, 1f, 1f);

        public TriList normal = new TriList();
        public TriList wire = new TriList();

        Dictionary<string, QuadList> normalq = new Dictionary<string, QuadList>();
        Dictionary<string, QuadList> animatedq = new Dictionary<string, QuadList>();
        QuadList wireq = new QuadList();

        Dictionary<string, QuadList> flagq = new Dictionary<string, QuadList>();

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

        public MGQuadBlock(SkyBox sb)
        {
            normal.textureEnabled = false;
            normal.scrollingEnabled = false;

            for (int i = 0; i < sb.faces.Count; i++)
            {
                List<VertexPositionColorTexture> tri = new List<VertexPositionColorTexture>();
                tri.Add(MGConverter.ToVptc(sb.verts[sb.faces[i].X], new CTRFramework.Shared.Vector2b(0, 0)));
                tri.Add(MGConverter.ToVptc(sb.verts[sb.faces[i].Y], new CTRFramework.Shared.Vector2b(0, 0)));
                tri.Add(MGConverter.ToVptc(sb.verts[sb.faces[i].Z], new CTRFramework.Shared.Vector2b(0, 0)));

                normal.PushTri(tri);
            }

            normal.Seal();

            wire = new TriList(normal);
            wire.SetColor(Color.Red);   
        }



        public MGQuadBlock(Scene s, Detail detail)
        {
            switch (detail)
            {
                case Detail.Low:
                    {
                        Console.WriteLine("doin low");

                        List<VertexPositionColorTexture> monolist = new List<VertexPositionColorTexture>();

                        for (int i = 0; i < s.quads.Count; i++)
                        {
                            //if (!s.quads[i].quadFlags.HasFlag(QuadFlags.InvisibleTriggers))
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


                                    foreach (QuadFlags fl in (QuadFlags[])Enum.GetValues(typeof(QuadFlags)))
                                    {
                                        if (s.quads[i].quadFlags.HasFlag(fl))
                                        {
                                            if (flagq.ContainsKey(fl.ToString()))
                                            {
                                                flagq[fl.ToString()].PushQuad(monolist);
                                            }
                                            else
                                            {
                                                QuadList ql = new QuadList(monolist, true, "flag");
                                                flagq.Add(fl.ToString(), ql);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        break;
                    }

                case Detail.Med:
                    {
                        Console.WriteLine("doin hi");

                        List<VertexPositionColorTexture> monolist = new List<VertexPositionColorTexture>();

                        for (int i = 0; i < s.quads.Count; i++)
                        {
                           // Console.WriteLine(i + "\\" + s.quads.Count );

                            //if (!s.quads[i].quadFlags.HasFlag(QuadFlags.InvisibleTriggers))
                            for (int j = 0; j < 4; j++)
                            {

                                List<CTRFramework.Vertex> vts = s.quads[i].GetVertexListq(s, j);
                                monolist.Clear();

                                if (vts != null)
                                {
                                    foreach (Vertex cv in vts)
                                        monolist.Add(MGConverter.ToVptc(cv, cv.uv));

                                    wireq.PushQuad(monolist);

                                    bool isAnimated = false;
                                    string texTag = "test";

                                    if (s.quads[i].ptrTexMid[j] != 0)
                                    {
                                        isAnimated = s.quads[i].tex[j].isAnimated;
                                        if (texTag != "00000000")
                                            texTag = s.quads[i].tex[j].midlods[2].Tag();
                                    }

                                    if (s.quads[i].quadFlags.HasFlag(QuadFlags.InvisibleTriggers))
                                    {
                                        if (flagq.ContainsKey("invis"))
                                        {
                                            flagq["invis"].PushQuad(monolist);
                                        }
                                        else
                                        {
                                            QuadList ql = new QuadList(monolist, true, "invis");
                                            flagq.Add("invis", ql);
                                        }
                                    }
                                    else
                                    {
                                        if (!isAnimated)
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
                        }

                        break;
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

            foreach (var ql in flagq)
                ql.Value.Seal();



            Console.WriteLine("numVerts " + numVerts);
            Console.WriteLine("numQuads " + numQuads);
            Console.WriteLine("numDrawCalls " + numDrawCalls);
        }


        public void Update(GameTime gameTime)
        {
            foreach (var ql in animatedq)
                ql.Value.Update(gameTime);
        }

        public void Render(GraphicsDeviceManager graphics, BasicEffect effect)
        {
            Samplers.SetToDevice(graphics, EngineSampler.Default);

            foreach (var ql in normalq)
                ql.Value.Render(graphics, effect);

            Samplers.SetToDevice(graphics, EngineSampler.Animated);

            foreach (var ql in animatedq)
                ql.Value.Render(graphics, effect);

            Samplers.SetToDevice(graphics, EngineSampler.Default);

            if (flagq.ContainsKey(((QuadFlags)(1<<Game1.currentflag)).ToString()))
                flagq[((QuadFlags)(1 << Game1.currentflag)).ToString()].Render(graphics, effect);

            if (!Game1.hide_invis)
            {
                effect.Alpha = 0.25f;

                if (flagq.ContainsKey("invis"))
                    flagq["invis"].Render(graphics, effect);

                effect.Alpha = 1f;
            }
        }

        public void RenderSky(GraphicsDeviceManager graphics, BasicEffect effect)
        {
            normal.Render(graphics, effect);
        }

        public void RenderWire(GraphicsDeviceManager graphics, BasicEffect effect)
        {        
            wire.Render(graphics, effect);
            wireq.Render(graphics, effect);
        }
    }
}