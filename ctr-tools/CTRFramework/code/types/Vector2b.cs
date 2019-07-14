using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace CTRFramework
{
    class Vector2b : IRead, IWrite, IByteArray
    {

        #region Public Fields

        [CategoryAttribute("Values"), DescriptionAttribute("X coordinate.")]
        public byte X
        {
            get { return x; }
            set { x = value; }
        }

        [CategoryAttribute("Values"), DescriptionAttribute("Y coordinate.")]
        public byte Y
        {
            get { return y; }
            set { y = value; }
        }

        #endregion

        #region Private Fields

        private byte x = 0;
        private byte y = 0;

        #endregion

        //constructor

        public Vector2b(BinaryReader br)
        {
            Read(br);
        }


        #region Interfaces

        public void Read(BinaryReader br)
        {
            x = br.ReadByte();
            y = br.ReadByte();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(ToByteArray());
        }

        public byte[] ToByteArray()
        {
            return new byte[] { x, y };
        }

        public string ToString(VecFormat format)
        {
            string fmt = "{0} {1} {2}";

            switch (format)
            {
                case VecFormat.CommaSeparated: fmt = "{0}, {1}"; break;
                case VecFormat.Braced: fmt = "({0}, {1})"; break;
            }

            return String.Format(fmt, x, y);
        }

        public override string ToString()
        {
            return ToString(VecFormat.Braced);
        }

        #endregion
    }
}
