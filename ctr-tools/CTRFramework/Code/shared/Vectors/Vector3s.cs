using System;
using System.ComponentModel;

namespace CTRFramework.Shared
{
    public class Vector3s : IRead, IEquatable<Vector3s>
    {
        #region ComponentModel
        [CategoryAttribute("Values"), DescriptionAttribute("X axis.")]
        public short X
        {
            get { return x; }
            set { x = value; }
        }

        [CategoryAttribute("Values"), DescriptionAttribute("Y axis.")]
        public short Y
        {
            get { return y; }
            set { y = value; }
        }

        [CategoryAttribute("Values"), DescriptionAttribute("Z axis.")]
        public short Z
        {
            get { return z; }
            set { z = value; }
        }
        #endregion

        private short x = 0;
        private short y = 0;
        private short z = 0;

        public static Vector3s Zero = new Vector3s(0);

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
        /// Compares to another Vector3s. Implements IEquatable interface.
        /// </summary>
        /// <param name="v">Vector3s to compare with.</param>
        /// <returns>True if equals.</returns>
        public bool Equals(Vector3s v)
        {
            if (v.X == X && v.Y == Y && v.Z == Z)
                return true;

            return false;
        }

        /// <summary>
        /// Returns new copy of Vector3s.
        /// </summary>
        /// <returns>Vector3s object.</returns>
        public Vector3s Clone()
        {
            return new Vector3s(x, y, z);
        }
    }
}