using System.ComponentModel;

namespace CTRFramework.Shared
{
    public class Pose : IReadWrite
    {
        #region ComponentModel
        [CategoryAttribute("Values"), DescriptionAttribute("Position vector.")]
        public Vector3s Position
        {
            get { return position; }
            set { position = value; }
        }

        [CategoryAttribute("Values"), DescriptionAttribute("Rotation vector.")]
        public Vector3s Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        #endregion

        private Vector3s position;
        private Vector3s rotation;

        public static Pose Zero = new Pose(Vector3s.Zero, Vector3s.Zero);

        public Pose()
        {
        }

        public Pose(BinaryReaderEx br)
        {
            Read(br);
        }

        public Pose(Vector3s pos, Vector3s ang)
        {
            position = pos;
            rotation = ang;
        }

        public void Read(BinaryReaderEx br)
        {
            position = new Vector3s(br);
            rotation = new Vector3s(br);
        }

        public void Write(BinaryWriterEx bw)
        {
            position.Write(bw);
            rotation.Write(bw);
        }

        public override string ToString()
        {
            return $"Pos: {position} Rot: {rotation}";
        }
    }
}