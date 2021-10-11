using System;
using System.Drawing;

namespace CTRFramework.Shared
{
    public class Vector4b : IRead, IEquatable<Vector4b>
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


        public Vector4b(byte x, byte y, byte z, byte w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
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
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            X = br.ReadByte();
            Y = br.ReadByte();
            Z = br.ReadByte();
            W = br.ReadByte();
        }

        public void Write(BinaryWriterEx bw)
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

        public bool Equals(Vector4b v)
        {
            if (v.X != X)
                return false;

            if (v.Y != Y)
                return false;

            if (v.Z != Z)
                return false;

            if (v.W != W)
                return false;

            return true;
        }

        public override string ToString()
        {
            return ToString(VecFormat.Braced);
        }
    }
}
