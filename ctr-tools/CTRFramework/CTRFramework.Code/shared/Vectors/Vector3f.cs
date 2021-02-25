using System;
using System.ComponentModel;

namespace CTRFramework.Shared
{
    public class Vector3f : IReadWrite
    {

        [CategoryAttribute("Values"), DescriptionAttribute("Position of the model.")]
        public float X
        {
            get { return x; }
            set { x = value; }
        }

        [CategoryAttribute("Values"), DescriptionAttribute("Position of the model.")]
        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        [CategoryAttribute("Values"), DescriptionAttribute("Position of the model.")]
        public float Z
        {
            get { return z; }
            set { z = value; }
        }

        private float x = 0;
        private float y = 0;
        private float z = 0;


        public Vector3f(float xx)
        {
            x = xx;
            y = xx;
            z = xx;
        }

        public Vector3f(float xx, float yy, float zz)
        {
            x = xx;
            y = yy;
            z = zz;
        }

        public Vector3f(BinaryReaderEx br)
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

        public Vector3f Move(Vector3s v)
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


        public Vector3f Clone()
        {
            return new Vector3f(x, y, z);
        }
    }
}
