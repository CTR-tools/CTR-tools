using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CTRFramework.Vram;
using CTRFramework.Shared;
using System.Numerics;

namespace CTRFramework
{
    public class CtrQuad
    {
        public Vertex[] Vertices;
        public byte DrawingOrder = 0;
        public RotateFlipType RotateFlipType = RotateFlipType.None;
        public FaceMode FaceMode = FaceMode.Normal;
        public Vector2 FaceNormal = Vector2.Zero;
        public CtrTex Texture;
        public int numVerts => FaceMode == FaceMode.Normal ? 4 : 3;

        public string ToObj(ref int num)
        {
            if (Vertices == null || Vertices.Length <= 4)
            {
                Helpers.Panic(this, PanicType.Warning, "No vertices in quad.");
                //return "";
            }

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < numVerts; i++)
                sb.AppendLine(Vertices[i].ToObj());

            if (Texture != null)
                if (Texture.lod2 != null)
                    for (int i = 0; i < numVerts; i++)
                        sb.AppendLine($"vt {Texture.lod2.normuv[i].X} {-Texture.lod2.normuv[i].Y}");

            sb.Append($"usemtl {(Texture.lod2 == null ? "default" : Texture.lod2.Tag)}");

            sb.AppendLine($"f {num + 1} {num + 2} {num + 3}");
            
            if (FaceMode == FaceMode.Normal)
                sb.AppendLine($"f {num + 3} {num + 2} {num + 4}");

            num += numVerts;

            return sb.ToString();
        }
    }
}