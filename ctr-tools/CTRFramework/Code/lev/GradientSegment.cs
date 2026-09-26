using System.Collections.Generic;

namespace CTRFramework.Shared
{
    public class GradientPoint
    {
        public short Value { get; set; } = 0;
        public Vector4b Color { get; set; } = new Vector4b(0, 0, 0, 0);

        public GradientPoint() { }

        public override string ToString()
        {
            return $"GradPoint: {Value}, {Color}";
        }
    }

    public class GradientSegment : IReadWrite
    {
        public GradientPoint From { get; set; }
        public GradientPoint To { get; set; }

        public GradientSegment()
        {
        }

        public GradientSegment(BinaryReaderEx br) => Read(br);

        public static GradientSegment FromReader(BinaryReaderEx br) => new GradientSegment(br);

        public void Read(BinaryReaderEx br)
        {
            From = new GradientPoint();
            To = new GradientPoint();

            From.Value = br.ReadInt16();
            To.Value = br.ReadInt16();
            From.Color = br.ReadVector4b();
            To.Color = br.ReadVector4b();
        }

        public void Write(BinaryWriterEx bw, List<PsxPtr> patchTable = null)
        {
            bw.Write(From.Value);
            bw.Write(To.Value);
            From.Color.Write(bw);
            To.Color.Write(bw);
        }

        public void Flip()
        {
            var buf = From;
            From = To;
            To = buf;
        }

        public override string ToString() => $"Gradient: from [{From}], to [{To}]";
    }
}