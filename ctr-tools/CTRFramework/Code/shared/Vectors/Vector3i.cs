using System;
using System.ComponentModel;

namespace CTRFramework.Shared
{
    public class Vector3i : IRead, IEquatable<Vector3i>
    {
        #region ComponentModel
        [CategoryAttribute("Values"), DescriptionAttribute("X axis.")]
        public int X
        {
            get { return x; }
            set { x = value; }
        }

        [CategoryAttribute("Values"), DescriptionAttribute("Y axis.")]
        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        [CategoryAttribute("Values"), DescriptionAttribute("Z axis.")]
        public int Z
        {
            get { return z; }
            set { z = value; }
        }
        #endregion

        private int x = 0;
        private int y = 0;
        private int z = 0;

        public static Vector3i Zero = new Vector3i(0);

        public Vector3i(int xx)
        {
            x = xx;
            y = xx;
            z = xx;
        }

        public Vector3i(int xx, int yy, int zz)
        {
            x = xx;
            y = yy;
            z = zz;
        }

        public Vector3i(BinaryReaderEx br)
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
            bw.Write(x);
            bw.Write(y);
            bw.Write(z);
        }

        public override string ToString()
        {
            return ToString(VecFormat.Braced);
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

        /// <summary>
        /// Compares to another Vector3i. Implements IEquatable interface.
        /// </summary>
        /// <param name="v">Vector3i to compare with.</param>
        /// <returns>True if equals.</returns>
        public bool Equals(Vector3i v)
        {
            if (v.X == X && v.Y == Y && v.Z == Z)
                return true;

            return false;
        }

        /// <summary>
        /// Returns new copy of Vector3i.
        /// </summary>
        /// <returns>Vector3i object.</returns>
        public Vector3i Clone()
        {
            return new Vector3i(x, y, z);
        }
    }
}