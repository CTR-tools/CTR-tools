using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace CTRFramework
{
    public class Vertex : IReadWrite
    {
        public Vector3 coord;
        public Vector4b color;
        public Vector4b color_morph;

        public Vector4b color_target;
        public Vector2b uv; //not used by CTR, added for convenience

        public Vertex()
        {
        }

        public Vertex(BinaryReaderEx br)
        {
            Read(br);
        }

        public void SetColor(Vector4b col, Vcolor mode)
        {
            switch (mode)
            {
                case Vcolor.Default: color = col; break;
                case Vcolor.Morph: color_morph = col; break;
            }
        }

        public void ReadShort(BinaryReaderEx br)
        {
            coord = br.ReadVector3sPadded(1 / 100f);
            color = new Vector4b(br);
        }

        public void Read(BinaryReaderEx br)
        {
            coord = br.ReadVector3sPadded(1 / 100f);
            color = new Vector4b(br);
            color_morph = new Vector4b(br);
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.WriteVector3sPadded(coord, 1 / 100f);
            color.Write(bw);
            color_morph.Write(bw);
        }

        public string ToObj()
        {
            return $"v {coord.X} {coord.Y} {coord.Z} {(color.X / 255f).ToString("0.###")} {(color.Y / 255f).ToString("0.###")} {(color.Z / 255f).ToString("0.###")}";
        }
    }
}