using System;
using System.ComponentModel;

namespace CTRFramework.Shared
{
    public class Vector3f : IRead, IEquatable<Vector3f>
    {
        #region ComponentModel
        [CategoryAttribute("Values"), DescriptionAttribute("X axis.")]
        public float X
        {
            get { return x; }
            set { x = value; }
        }

        [CategoryAttribute("Values"), DescriptionAttribute("Y axis.")]
        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        [CategoryAttribute("Values"), DescriptionAttribute("Z axis.")]
        public float Z
        {
            get { return z; }
            set { z = value; }
        }
        #endregion

        private float x = 0;
        private float y = 0;
        private float z = 0;

        public static Vector3f Zero = new Vector3f(0);

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

        public static Vector3f operator +(Vector3f a, float b)
        {
            return new Vector3f(a.X + b, a.Y + b, a.Z + b);
        }

        public static Vector3f operator -(Vector3f a, float b)
        {
            return new Vector3f(a.X - b, a.Y - b, a.Z - b);
        }

        public static Vector3f operator *(Vector3f a, float b)
        {
            return new Vector3f(a.X * b, a.Y * b, a.Z * b);
        }
        public static Vector3f operator /(Vector3f a, float b)
        {
            if (b == 0)
                throw new DivideByZeroException();

            return new Vector3f(a.X / b, a.Y / b, a.Z / b);
        }

        public static Vector3f operator +(Vector3f a, Vector3f b)
        {
            return new Vector3f(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3f operator -(Vector3f a, Vector3f b)
        {
            return new Vector3f(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector3f operator *(Vector3f a, Vector3f b)
        {
            return new Vector3f(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static Vector3f operator /(Vector3f a, Vector3f b)
        {
            if (b.X == 0 || b.Y == 0 || b.z == 0)
                throw new DivideByZeroException();

            return new Vector3f(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        public bool Equals(Vector3f v)
        {
            if (v.X != X)
                return false;

            if (v.Y != Y)
                return false;

            if (v.Z != Z)
                return false;

            return true;
        }

        public void Maximize(Vector3f v)
        {
            X = Math.Max(X, v.X);
            Y = Math.Max(Y, v.Y);
            Z = Math.Max(Z, v.Z);
        }

        public void Minimize(Vector3f v)
        {
            X = Math.Min(X, v.X);
            Y = Math.Min(Y, v.Y);
            Z = Math.Min(Z, v.Z);
        }

        public Vector3s ToVector3s()
        {
            return new Vector3s(
                (short)Math.Round(X),
                (short)Math.Round(Y),
                (short)Math.Round(Z)
                );
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
