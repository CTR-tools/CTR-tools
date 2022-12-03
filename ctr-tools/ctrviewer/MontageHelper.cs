using System;
using System.Collections.Generic;
using CTRFramework;
using CTRFramework.Vram;
using CTRFramework.Shared;
using ctrviewer.Engine.Render;
using Microsoft.Xna.Framework;
using VPCT = Microsoft.Xna.Framework.Graphics.VertexPositionColorTexture;
using ctrviewer.Engine;
using static System.Net.Mime.MediaTypeNames;
using System.Xml;

namespace ctrviewer
{
    public class MontageHelper
    {
        public static void LoadQuad(MainEngine eng, QuadBlock qb)
        {
            ContentVault.AddModel("montage", MontageHelper.FromQuadBlock(qb));
        }

        public static TriListCollection FromQuadBlock(QuadBlock qb)
        {
            var collection = new TriListCollection();

            if (qb.tex is null)
                return null;
            
            if (qb.tex.Count != 4)
                return null;

            foreach (var tex in qb.tex)
                if (tex is null)
                    return null;

            collection.AddRange(GetHiQuad(0, 0, qb.tex[0]));
            collection.AddRange(GetHiQuad(0, 1, qb.tex[1]));
            collection.AddRange(GetHiQuad(1, 0, qb.tex[2]));
            collection.AddRange(GetHiQuad(1, 1, qb.tex[3]));

            return collection;
        }


        //quad is non sealed here, be aware
        public static TriList GetQuad(Vector3 move, int index, TextureLayout tl)
        {
            var trilist = new TriList();

            trilist.PushQuad(
                new List<VPCT>() {
                    new VPCT(new Vector3(0, 0, 0), Color.Gray /** (index / 15f)*/, tl.normuv[0] / 255f),
                    new VPCT(new Vector3(1, 0, 0), Color.Gray /** (index / 15f)*/, tl.normuv[1] / 255f),
                    new VPCT(new Vector3(0, 1, 0), Color.Gray /** (index / 15f)*/, tl.normuv[2] / 255f),
                    new VPCT(new Vector3(1, 1, 0), Color.Gray /** (index / 15f)*/, tl.normuv[3] / 255f),
                    }
                );

            trilist.Move(move);

            return trilist;
        }

        public static List<TriList> GetHiQuad(int x, int y, CtrTex tex)
        {
            var trilist = new List<TriList>();

            float size = 1.0f;
            float width = size / 4f;
            float height = size / 4f;
            
            if (tex is null || tex.hi is null)
                return null;

            for (int i = 0; i < tex.hi.Count; i++)
            {
                int qx = i % 4;
                int qy = i / 4;

                var tri = GetQuad(new Vector3(x * 4 + qx * size, y * 4 + qy * size, 0), i, tex.hi[i]);

                tri.textureName = tex.hi[i].Tag;
                tri.textureEnabled = true;

                tri.Seal();

                trilist.Add(tri);
            }

            return trilist;
        }
    }
}