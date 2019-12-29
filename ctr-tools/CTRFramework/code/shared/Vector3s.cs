using System;
using System.ComponentModel;
using System.IO;

namespace CTRFramework.Shared
{
    public class Vector3s : IRead, IWrite
    {

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

        private short x = 0;
        private short y = 0;
        private short z = 0;


        public Vector3s(short xx)
        {
            x = xx;
            y = xx;
            z = xx;
        }

        public Vector3s(short xx, short yy, short zz)
        {
            x = xx;
            y = yy;
            z = zz;
        }

        public Vector3s(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            x = br.ReadInt16();
            y = br.ReadInt16();
            z = br.ReadInt16();
        }

        public void Write(BinaryWriterEx bw)
        {
            bw.Write(X);
            bw.Write(Y);
            bw.Write(Z);
        }

        public Vector3s Move(Vector3s v)
        {
            x += v.X;
            y += v.Y;
            z += v.Z;

            return this;
        }

        public string ToString(VecFormat format)
        {
            return ToString(format, 0);
        }

        public string ToString(VecFormat format, int inc)
        {
            string fmt = "{0} {1} {2}";

            switch (format)
            {
                case VecFormat.CommaSeparated: fmt = "{0}, {1}, {2}"; break;
                case VecFormat.Braced: fmt = "({0}, {1}, {2})"; break;
            }

            return String.Format(fmt, x + inc, y + inc, z + inc);
        }


        public override string ToString()
        {
            return ToString(VecFormat.Braced);
        }

    }
}
