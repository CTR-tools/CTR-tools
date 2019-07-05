using System;
using System.IO;
using System.ComponentModel;

namespace model_reader
{
    public class Vector3s
    {   
        private short x;
        private short y;
        private short z;

        [CategoryAttribute("Values"), DescriptionAttribute("Position of the model.")]
        public short X
        {
            get { return x; }
            set { x = value; }
        }

        [CategoryAttribute("Values"), DescriptionAttribute("Position of the model.")]
        public short Y
        {
            get { return y; }
            set { y = value; }
        }

        [CategoryAttribute("Values"), DescriptionAttribute("Position of the model.")]
        public short Z
        {
            get { return z; }
            set { z = value; }
        }

        public Vector3s(short xx, short yy, short zz)
        {
            x = xx;
            y = yy;
            z = zz;
        }

        public Vector3s(BinaryReader br)
        {
            x = br.ReadInt16();
            y = br.ReadInt16();
            z = br.ReadInt16();
        }

        /*
        public string ToObjVertex()
        {
            return "v " + X + " " + Y + " " + Z;
        }
        */

        public override string ToString()
        {
            return String.Format("{0}; {1}; {2}", X, Y, Z);
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(X);
            bw.Write(Y);
            bw.Write(Z);
        }
    }
}
