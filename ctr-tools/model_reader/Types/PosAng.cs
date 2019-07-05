using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace model_reader
{
    public class PosAng
    {
        public Vector3s Position;
        public Vector3s Angle;

        public PosAng(Vector3s pos, Vector3s ang)
        {
            Position = pos;
            Angle = ang;
        }

        public PosAng(BinaryReader br)
        {
            Position = new Vector3s(br);
            Angle = new Vector3s(br);
        }

        public override string ToString()
        {
            return "Pos: " + Position.ToString() + " Ang: " + Angle.ToString();
        }
    }
}
