using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace model_reader
{
    class PosAng
    {
        public Vector3s Position;
        public Vector3s Angle;

        public PosAng(Vector3s pos, Vector3s ang)
        {
            Position = pos;
            Angle = ang;
        }

        public override string ToString()
        {
            return "Pos: " + Position.ToString() + " Ang: " + Angle.ToString();
        }
    }
}
