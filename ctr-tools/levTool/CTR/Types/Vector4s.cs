using System;
using System.IO;
using System.ComponentModel;

namespace CTRtools
{
    public class Vector4s
    {
        private short x;
        private short y;
        private short z;
        private short w;

        [CategoryAttribute("Values")]
        public short X
        {
            get { return x; }
            set { x = value; }
        }

        [CategoryAttribute("Values")]
        public short Y
        {
            get { return y; }
            set { y = value; }
        }

        [CategoryAttribute("Values")]
        public short Z
        {
            get { return z; }
            set { z = value; }
        }

        [CategoryAttribute("Values")]
        public short W
        {
            get { return w; }
            set { w = value; }
        }

        public Vector4s(short xx, short yy, short zz, short ww)
        {
            x = xx;
            y = yy;
            z = zz;
            w = ww;
        }

        public Vector4s(BinaryReader br)
        {
            x = br.ReadInt16();
            y = br.ReadInt16();
            z = br.ReadInt16();
            w = br.ReadInt16();
        }

        /*
        public string ToObjVertex()
        {
            return "v " + X + " " + Y + " " + Z;
        }
        */

        public override string ToString()
        {
            return String.Format("{0}; {1}; {2}; {3}", x, y, z, w);
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(X);
            bw.Write(Y);
            bw.Write(Z);
            bw.Write(W);
        }
    }
}
