using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace CTRFramework
{
    public class Vertex : IReadWrite
    {
        public Vector3 Position;
        public Vector4b Color;
        public Vector4b MorphColor;

        public Vector4b color_target;
        public Vector2 uv; //not used by CTR, added for convenience

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
                case Vcolor.Default: Color = col; break;
                case Vcolor.Morph: MorphColor = col; break;
            }
        }

        public virtual void Read(BinaryReaderEx br)
        {
            Position = br.ReadVector3sPadded(1 / 100f);
            Color = new Vector4b(br);
            MorphColor = new Vector4b(br);
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.WriteVector3sPadded(Position, 1 / 100f);
            Color.Write(bw);
            MorphColor.Write(bw);
        }

        public string ToObj(float scale = 1.0f)
        {
            return $"v {Position.X * scale} {Position.Y * scale} {Position.Z * scale} {(Color.X / 255f).ToString("0.###")} {(Color.Y / 255f).ToString("0.###")} {(Color.Z / 255f).ToString("0.###")}";
        }
    }

    public class VertexShort : Vertex
    {
        public VertexShort(BinaryReaderEx br) : base(br)
        {
        }

        public override void Read(BinaryReaderEx br)
        {
            Position = br.ReadVector3sPadded(1 / 100f);
            Color = new Vector4b(br);
        }
    }
}