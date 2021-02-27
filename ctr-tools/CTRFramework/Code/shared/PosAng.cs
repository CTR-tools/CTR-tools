namespace CTRFramework.Shared
{
    public class PosAng : IReadWrite
    {
        public Vector3s Position;
        public Vector3s Angle;

        public PosAng()
        {
        }

        public PosAng(BinaryReaderEx br)
        {
            Read(br);
        }

        public PosAng(Vector3s pos, Vector3s ang)
        {
            Position = pos;
            Angle = ang;
        }

        public void Read(BinaryReaderEx br)
        {
            Position = new Vector3s(br);
            Angle = new Vector3s(br);
        }

        public void Write(BinaryWriterEx bw)
        {
            Position.Write(bw);
            Angle.Write(bw);
        }

        public override string ToString()
        {
            return "Pos: " + Position.ToString() + " Ang: " + Angle.ToString();
        }

        public static PosAng Default
        {
            get { return new PosAng(new Vector3s(0, 0, 0), new Vector3s(0, 0, 0)); }
        }
    }
}
