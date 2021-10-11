using System;
using System.Drawing;

namespace CTRFramework.Shared
{
    public class Vector3b : IRead, IEquatable<Vector3b>
    {
        public byte X = 0;
        public byte Y = 0;
        public byte Z = 0;

        public Vector3b()
        {
        }

        public Vector3b(byte x)
        {
            X = x;
            Y = x;
            Z = x;
        }

        public Vector3b(byte x, byte y, byte z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public void Scale(float x)
        {
            X = (byte)(X * x);
            Y = (byte)(Y * x);
            Z = (byte)(Z * x);
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

        public bool Equals(Vector3b v)
        {
            if (v.X != X)
                return false;

            if (v.Y != Y)
                return false;

            if (v.Z != Z)
                return false;

            return true;
        }

        public void BiteBits(byte mask)
        {
            X = (byte)(X & ~mask);
            Y = (byte)(Y & ~mask);
            Z = (byte)(Z & ~mask);
        }

        public string ToString(VecFormat format)
        {
            switch (format)
            {
                case VecFormat.CommaSeparated: return $"{X}, {Y}, {Z}";
                case VecFormat.Braced: return $"({X}, {Y}, {Z})";
                case VecFormat.Hex: return $"{X.ToString("X2")}, {Y.ToString("X2")}, {Z.ToString("X2")}";
                default: throw new NotSupportedException("Unknown VecFormat.");
            }
        }

        public override string ToString()
        {
            return ToString(VecFormat.Braced);
        }
    }
}