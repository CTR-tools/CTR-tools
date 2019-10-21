using System;
using System.Drawing;
using System.IO;

namespace CTRFramework.Shared
{
    public class Vector4b
    {
        public byte X;
        public byte Y;
        public byte Z;
        public byte W;


        public void Scale(float x)
        {
            X = (byte)(X * x);
            Y = (byte)(Y * x);
            Z = (byte)(Z * x);
            W = (byte)(W * x);
        }

        public void Scale(float x, float y, float z, float w)
        {
            X = (byte)(X * x);
            Y = (byte)(Y * y);
            Z = (byte)(Z * z);
            W = (byte)(W * w);
        }


        public Vector4b(Color c)
        {
            X = c.R;
            Y = c.G;
            Z = c.B;
            W = 0;
        }

        public Vector4b(uint a)
        {
            X = (byte)(a >> 24 & 0xFF);
            Y = (byte)(a >> 16 & 0xFF);
            Z = (byte)(a >> 8 & 0xFF);
            W = (byte)(a & 0xFF);
        }

        public Vector4b(BinaryReaderEx br)
        {
            X = br.ReadByte();
            Y = br.ReadByte();
            Z = br.ReadByte();
            W = br.ReadByte();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(X);
            bw.Write(Y);
            bw.Write(Z);
            bw.Write(W);
        }

        public string ToString(VecFormat format)
        {
            string fmt = "{0} {1} {2} {3}";

            switch (format)
            {
                case VecFormat.CommaSeparated: fmt = "{0}, {1}, {2}, {3}"; break;
                case VecFormat.Braced: fmt = "({0}, {1}, {2}, {3})"; break;
            }

            return String.Format(fmt, X, Y, Z, W);
        }

        public override string ToString()
        {
            return ToString(VecFormat.Braced);
        }
    }
}
