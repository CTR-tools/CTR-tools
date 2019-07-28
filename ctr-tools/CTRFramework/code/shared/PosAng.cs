using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CTRFramework.Shared
{
    public class PosAng : IRead
    {
        public Vector3s Position;
        public Vector3s Angle;

        public PosAng()
        {
        }

        public PosAng(BinaryReader br)
        {
            Read(br);
        }

        public PosAng(Vector3s pos, Vector3s ang)
        {
            Position = pos;
            Angle = ang;
        }

        public void Read(BinaryReader br)
        {
            Position = new Vector3s(br);
            Angle = new Vector3s(br);
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
