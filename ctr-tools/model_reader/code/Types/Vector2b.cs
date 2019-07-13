using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace model_reader
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
            get { return x; }
            set { x = value; }
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

        /// <inheritdoc/>
        public void Read(BinaryReader br)
        {
            x = br.ReadByte();
            y = br.ReadByte();
        }

        /// <inheritdoc/>
        public void Write(BinaryWriter bw)
        {
            bw.Write(ToByteArray());
        }

        /// <inheritdoc/>
        public byte[] ToByteArray()
        {
            return new byte[] { x, y };
        }


        public override string ToString()
        {
            return String.Format("({0}, {1})", x, y);
        }

        #endregion
    }
}
