using System;
using System.ComponentModel;

namespace CTRFramework.Shared
{
    public class Vector2s : IReadWrite
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

        private short x = 0;
        private short y = 0;

        public Vector2s(short xx)
        {
            x = xx;
            y = xx;
        }

        public Vector2s(short xx, short yy)
        {
            x = xx;
            y = yy;
        }

        public Vector2s(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            x = br.ReadInt16();
            y = br.ReadInt16();
        }

        public void Write(BinaryWriterEx bw)
        {
            bw.Write(X);
            bw.Write(Y);
        }

        public Vector2s Move(Vector2s v)
        {
            x += v.X;
            y += v.Y;

            return this;
        }

        public string ToString(VecFormat format)
        {
            return ToString(format, 0);
        }

        public string ToString(VecFormat format, int inc)
        {
            string fmt = "{0} {1}";

            switch (format)
            {
                case VecFormat.CommaSeparated: fmt = "{0}, {1}"; break;
                case VecFormat.Braced: fmt = "({0}, {1})"; break;
            }

            return String.Format(fmt, x + inc, y + inc);
        }


        public override string ToString()
        {
            return ToString(VecFormat.Braced);
        }

    }
}
