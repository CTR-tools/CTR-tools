using CTRFramework;
using CTRFramework.Shared;
using CTRFramework.Vram;
using ctrviewer.Engine;
using ctrviewer.Engine.Render;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ctrviewer.Loaders
{
    public class CrashTeamRacingLoader : MGLevel
    {
        public CrashTeamRacingLoader(CtrScene scene, Detail detail) => LoadCrashTeamRacingScene(scene, detail);

        public static MGLevel FromScene(CtrScene scene, Detail detail) => new CrashTeamRacingLoader(scene, detail);

        private void LoadCrashTeamRacingScene(CtrScene scene, Detail detail)
        {
            if (scene.visdata.Count > 0)
                boundingBox = new Microsoft.Xna.Framework.BoundingBox(
                    DataConverter.ToVector3(scene.visdata[0].bbox.minf),
                    DataConverter.ToVector3(scene.visdata[0].bbox.maxf)
                    );

            var monolist = new List<VertexPositionColorTexture>();
            var vts = new List<Vertex>();

            switch (detail)
            {
                case Detail.Low:
                    {
                        Helpers.Panic(this, PanicType.Info, "doin low");

                        foreach (var qb in scene.quads)
                        {
                            monolist.Clear();
                            vts = qb.GetVertexListq(scene.verts, -1);

                            if (vts != null)
                            {
                                foreach (Vertex cv in vts)
                                    monolist.Add(DataConverter.ToVptc(cv, cv.uv));

                                string texTag = qb.texlow.Tag;

                                foreach (QuadFlags fl in (QuadFlags[])Enum.GetValues(typeof(QuadFlags)))
                                {
                                    if (qb.quadFlags.HasFlag(fl))
                                        Push(Trilists, fl.ToString(), monolist, TriListType.Flag, null, "flag");
                                }

                                if (qb.visNodeFlags.HasFlag(VisNodeFlags.Water))
                                {
                                    Push(Trilists, "water", monolist, TriListType.Water);
                                    continue;
                                }

                                if (qb.visNodeFlags.HasFlag(VisNodeFlags.Hidden))
                                {
                                    Push(Trilists, "invis", monolist, TriListType.Flag);
                                    continue;
                                }

                                if (ContentVault.alphalist.Contains(texTag))
                                {
                                    Push(Trilists, "mesh", monolist, TriListType.Alpha, null, texTag);
                                }
                                else
                                {
                                    Push(Trilists, "mesh", monolist, TriListType.Basic, null, texTag);
                                }
                            }
                        }

                        break;
                    }

                case Detail.Med:
                    {
                        Helpers.Panic(this, PanicType.Info, "doin med");

                        foreach (var qb in scene.quads)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                monolist.Clear();
                                vts = qb.GetVertexListq(scene.verts, j);

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

                                    if (qb.visNodeFlags.HasFlag(VisNodeFlags.Water))
                                    {
                                        Push(Trilists, "water", monolist, TriListType.Water);
                                        continue;
                                    }

                                    if (qb.visNodeFlags.HasFlag(VisNodeFlags.Hidden))
                                    {
                                        Push(flagq, "invis", monolist, TriListType.Flag, BlendState.Additive, "test");
                                        continue;
                                    }

                                    bool isAlpha = ContentVault.alphalist.Contains(texTag);

                                    Push(Trilists, "mesh", monolist,
                                        (isAnimated ? TriListType.Animated : (isAlpha ? TriListType.Alpha : TriListType.Basic)),
                                        blendState == BlendState.Additive ? BlendState.Additive : (isAlpha ? BlendState.AlphaBlend : BlendState.Opaque)//isAlpha ? (blendState == BlendState.Additive ? blendState : BlendState.Opaque) : BlendState.Opaque
                                       , texTag
                                        );

                                    if (qb.doubleSided)
                                        Trilists["mesh"].CullingEnabled = false;

                                    if (isAnimated)
                                        foreach (var ql in Trilists)
                                            if (ql.Value.type == TriListType.Animated)
                                                Trilists["mesh"].ScrollingEnabled = true;

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

                        foreach (var qb in scene.quads)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                monolist.Clear();
                                vts = qb.GetVertexListq(scene.verts, j);

                                var subdiv = Helpers.Subdivide(vts);

                                var evenmore = new List<Vertex>();
                                evenmore.AddRange(Helpers.Subdivide(new List<Vertex> { subdiv[0], subdiv[1], subdiv[2], subdiv[3] }));
                                evenmore.AddRange(Helpers.Subdivide(new List<Vertex> { subdiv[4], subdiv[5], subdiv[6], subdiv[7] }));
                                evenmore.AddRange(Helpers.Subdivide(new List<Vertex> { subdiv[8], subdiv[9], subdiv[10], subdiv[11] }));
                                evenmore.AddRange(Helpers.Subdivide(new List<Vertex> { subdiv[12], subdiv[13], subdiv[14], subdiv[15] }));

                                if (vts != null)
                                {
                                    foreach (var x in evenmore)
                                        monolist.Add(DataConverter.ToVptc(x, Vector2.Zero));

                                    string texTag = (qb?.tex[j]?.lod2 is null ? "test" : qb.tex[j].lod2.Tag);

                                    Push(Trilists, texTag, monolist, TriListType.Basic, BlendState.Opaque);
                                }
                            }
                        }

                        break;
                    }
            }

            //move double sided to the end
            foreach (var trilist in Trilists.ToList())
            {
                if (!trilist.Value.CullingEnabled)
                {
                    var value = trilist.Value;
                    var key = trilist.Key;
                    Trilists.Remove(key);
                    Trilists.Add(key, value);
                }
            }

            Seal();
        }
    }
}