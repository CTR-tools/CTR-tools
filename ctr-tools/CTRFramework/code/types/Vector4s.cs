using System;
using System.IO;
using System.ComponentModel;

namespace CTRFramework
{
    public class Vector4s : IRead, IWrite
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
            Read(br);
        }

        public void Read(BinaryReader br)
        {
            x = br.ReadInt16();
            y = br.ReadInt16();
            z = br.ReadInt16();
            w = br.ReadInt16();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(X);
            bw.Write(Y);
            bw.Write(Z);
            bw.Write(W);
        }

        /*
        public string ToObjVertex()
        {
            return "v " + X + " " + Y + " " + Z;
        }
        */

        public string ToString(VecFormat format)
        {
            string fmt = "{0} {1} {2}";

            switch (format)
            {
                case VecFormat.CommaSeparated: fmt = "{0}, {1}, {2}"; break;
                case VecFormat.Braced: fmt = "({0}, {1}, {2})"; break;
            }

            return String.Format(fmt, x, y, z);
        }

        public override string ToString()
        {
            return ToString(VecFormat.Braced);
        }

    }
}
