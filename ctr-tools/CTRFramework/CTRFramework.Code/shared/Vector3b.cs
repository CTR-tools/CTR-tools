using System;
using System.Drawing;

namespace CTRFramework.Shared
{
    public class Vector3b : IReadWrite
    {
        public byte X;
        public byte Y;
        public byte Z;

        public void Scale(float x)
        {
            X = (byte)(X * x);
            Y = (byte)(Y * x);
            Z = (byte)(Z * x);
        }

        public Vector3b(byte x, byte y, byte z)
        {
            X = (byte)(X * x);
            Y = (byte)(Y * y);
            Z = (byte)(Z * z);
        }

        public void Scale(float x, float y, float z)
        {
            X = (byte)(X * x);
            Y = (byte)(Y * y);
            Z = (byte)(Z * z);
        }


        public Vector3b(Color c)
        {
            X = c.R;
            Y = c.G;
            Z = c.B;
        }


        public Vector3b(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            X = br.ReadByte();
            Y = br.ReadByte();
            Z = br.ReadByte();
        }

        public void Write(BinaryWriterEx bw)
        {
            bw.Write(X);
            bw.Write(Y);
            bw.Write(Z);
        }

        public string ToString(VecFormat format)
        {
            string fmt = "{0} {1} {2}";

            switch (format)
            {
                case VecFormat.CommaSeparated: fmt = "{0}, {1}, {2}"; break;
                case VecFormat.Braced: fmt = "({0}, {1}, {2})"; break;
            }

            return String.Format(fmt, X, Y, Z);
        }

        public override string ToString()
        {
            return ToString(VecFormat.Braced);
        }
    }
}
