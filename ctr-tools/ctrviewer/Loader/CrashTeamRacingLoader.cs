using CTRFramework;
using CTRFramework.Shared;
using CTRFramework.Vram;
using ctrviewer.Engine;
using ctrviewer.Engine.Render;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ctrviewer.Loaders
{
    public class CrashTeamRacingLoader : MGLevel
    {
        public CrashTeamRacingLoader(Scene s, Detail detail)
        {
            LoadCrashTeamRacingScene(s, detail);
        }

        public static MGLevel FromScene(Scene s, Detail detail)
        {
            return new CrashTeamRacingLoader(s, detail);
        }

        private void LoadCrashTeamRacingScene(Scene s, Detail detail)
        {
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

                                if (qb.isWater)
                                {
                                    Push(Trilists, "water", monolist, TriListType.Water);
                                    continue;
                                }

                                if (qb.quadFlags.HasFlag(QuadFlags.InvisibleTriggers))
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

                                    if (qb.ptrTexMid[j] != PsxPtr.Zero)
                                    {
                                        if (qb.tex[j] != null)
                                        {
                                            isAnimated = qb.tex[j].isAnimated;
                                            if (texTag != "00000000")
                                                texTag = qb.tex[j].midlods[2].Tag;

                                            switch (qb.tex[j].midlods[2].blendingMode)
                                            {
                                                case BlendingMode.Additive: blendState = BlendState.Additive; break;
                                                case BlendingMode.Translucent: blendState = BlendState.AlphaBlend; break;
                                            }
                                        }
                                    }

                                    foreach (var fl in (QuadFlags[])Enum.GetValues(typeof(QuadFlags)))
                                    {
                                        if (qb.quadFlags.HasFlag(fl))
                                            Push(flagq, fl.ToString(), monolist, TriListType.Flag, BlendState.Additive, "flag");
                                    }

                                    if (qb.isWater)
                                    {
                                        Push(Trilists, "water", monolist, TriListType.Water);
                                        continue;
                                    }

                                    if (qb.quadFlags.HasFlag(QuadFlags.InvisibleTriggers))
                                    {
                                        Push(flagq, "invis", monolist, TriListType.Flag);
                                        continue;
                                    }

                                    bool isAlpha = ContentVault.alphalist.Contains(texTag);

                                    Push(Trilists, texTag, monolist,
                                        (isAnimated ? TriListType.Animated : (isAlpha ? TriListType.Alpha : TriListType.Basic)), blendState
                                        );

                                    if (isAnimated)
                                        foreach (var ql in Trilists)
                                            if (ql.Value.type == TriListType.Animated)
                                                Trilists[texTag].ScrollingEnabled = true;
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
