using CTRFramework;
using CTRFramework.Shared;
using CTRFramework.Vram;
using ctrviewer.Engine;
using ctrviewer.Engine.Render;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace ctrviewer.Loaders
{
    public class CrashTeamRacingLoader : MGLevel
    {
        public CrashTeamRacingLoader(CtrScene s, Detail detail)
        {
            LoadCrashTeamRacingScene(s, detail);
        }

        public static MGLevel FromScene(CtrScene s, Detail detail)
        {
            return new CrashTeamRacingLoader(s, detail);
        }

        private void LoadCrashTeamRacingScene(CtrScene s, Detail detail)
        {
            if (s.visdata.Count > 0)
                boundingBox = new Microsoft.Xna.Framework.BoundingBox(
                    DataConverter.ToVector3(s.visdata[0].bbox.numericMin),
                    DataConverter.ToVector3(s.visdata[0].bbox.numericMax)
                    );

            List<VertexPositionColorTexture> monolist = new List<VertexPositionColorTexture>();
            List<CTRFramework.Vertex> vts = new List<Vertex>();

            switch (detail)
            {
                case Detail.Low:
                    {
                        Helpers.Panic(this, PanicType.Info, "doin low");

                        foreach (QuadBlock qb in s.quads)
                        {
                            monolist.Clear();
                            vts = qb.GetVertexListq(s.verts, -1);

                            if (vts != null)
                            {
                                foreach (Vertex cv in vts)
                                    monolist.Add(DataConverter.ToVptc(cv, cv.uv));

                                TextureLayout t = qb.texlow;

                                string texTag = t.Tag;

                                foreach (QuadFlags fl in (QuadFlags[])Enum.GetValues(typeof(QuadFlags)))
                                {
                                    if (qb.quadFlags.HasFlag(fl))
                                        Push(Trilists, fl.ToString(), monolist, TriListType.Flag, null, "flag");
                                }

                                if (qb.visDataFlags.HasFlag(VisDataFlags.Water))
                                {
                                    Push(Trilists, "water", monolist, TriListType.Water);
                                    continue;
                                }

                                if (qb.visDataFlags.HasFlag(VisDataFlags.Hidden))
                                {
                                    Push(Trilists, "invis", monolist, TriListType.Flag);
                                    continue;
                                }

                                if (ContentVault.alphalist.Contains(texTag))
                                {
                                    Push(Trilists, texTag, monolist, TriListType.Alpha);
                                }
                                else
                                {
                                    Push(Trilists, texTag, monolist);
                                }
                            }
                        }

                        break;
                    }

                case Detail.Med:
                    {
                        Helpers.Panic(this, PanicType.Info, "doin med");

                        foreach (QuadBlock qb in s.quads)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                monolist.Clear();
                                vts = qb.GetVertexListq(s.verts, j);

                                if (vts != null)
                                {
                                    foreach (var vertex in vts)
                                        monolist.Add(DataConverter.ToVptc(vertex, vertex.uv));

                                    bool isAnimated = false;
                                    string texTag = "test";

                                    BlendState blendState = BlendState.Opaque;
                                    BlendingMode bmode = BlendingMode.Standard;

                                    if (qb.ptrTexMid[j] != PsxPtr.Zero)
                                    {
                                        if (qb.tex[j] != null)
                                        {
                                            isAnimated = qb.tex[j].isAnimated;
                                            texTag = qb.tex[j].lod2.Tag;
                                            bmode = qb.tex[j].lod2.blendingMode;


                                            switch (bmode)
                                            {
                                                case BlendingMode.Additive: blendState = BlendState.Additive; break;
                                            }
                                        }
                                    }

                                    foreach (var fl in (QuadFlags[])Enum.GetValues(typeof(QuadFlags)))
                                    {
                                        if (qb.quadFlags.HasFlag(fl))
                                            Push(flagq, fl.ToString(), monolist, TriListType.Flag, BlendState.Additive, "flag");
                                    }

                                    if (qb.visDataFlags.HasFlag(VisDataFlags.Water))
                                    {
                                        Push(Trilists, "water", monolist, TriListType.Water);
                                        continue;
                                    }

                                    if (qb.visDataFlags.HasFlag(VisDataFlags.Hidden))
                                    {
                                        Push(flagq, "invis", monolist, TriListType.Flag, BlendState.Additive, "test");
                                        continue;
                                    }

                                    bool isAlpha = ContentVault.alphalist.Contains(texTag);

                                    Push(Trilists, texTag, monolist,
                                        (isAnimated ? TriListType.Animated : (isAlpha ? TriListType.Alpha : TriListType.Basic)),
                                        blendState == BlendState.Additive ? BlendState.Additive : (isAlpha ? BlendState.AlphaBlend : BlendState.Opaque)//isAlpha ? (blendState == BlendState.Additive ? blendState : BlendState.Opaque) : BlendState.Opaque
                                        );

                                    if (isAnimated)
                                        foreach (var ql in Trilists)
                                            if (ql.Value.type == TriListType.Animated)
                                                Trilists[texTag].ScrollingEnabled = true;

                                    //foreach (var ql in Trilists)
                                    //    Trilists[texTag].textureEnabled = false;
                                }
                            }
                        }

                        break;
                    }

                case Detail.High:
                    {
                        Helpers.Panic(this, PanicType.Info, "doin hi");

                        foreach (QuadBlock qb in s.quads)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                monolist.Clear();
                                vts = qb.GetVertexListq(s.verts, j);

                                List<Vertex> subdiv = Helpers.Subdivide(vts);

                                List<Vertex> evenmore = new List<Vertex>();
                                evenmore.AddRange(Helpers.Subdivide(new List<Vertex> { subdiv[0], subdiv[1], subdiv[2], subdiv[3] }));
                                evenmore.AddRange(Helpers.Subdivide(new List<Vertex> { subdiv[4], subdiv[5], subdiv[6], subdiv[7] }));
                                evenmore.AddRange(Helpers.Subdivide(new List<Vertex> { subdiv[8], subdiv[9], subdiv[10], subdiv[11] }));
                                evenmore.AddRange(Helpers.Subdivide(new List<Vertex> { subdiv[12], subdiv[13], subdiv[14], subdiv[15] }));

                                if (vts != null)
                                {
                                    foreach (var x in evenmore)
                                        monolist.Add(DataConverter.ToVptc(x, Vector2.Zero));

                                    bool isAnimated = false;
                                    string texTag = (qb?.tex[j]?.lod2 == null ? "test" : qb.tex[j].lod2.Tag);

                                    Push(Trilists, texTag, monolist, TriListType.Basic, BlendState.Opaque);
                                }
                            }
                        }

                        break;
                    }
            }

            Seal();
        }
    }
}
