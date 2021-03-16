using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CTRFramework.Shared
{
    public class BinaryReaderEx : BinaryReader
    {
        public BinaryReaderEx(Stream stream) : base(stream)
        {
        }

        #region Stream position helpers

        /// <summary>
        /// Implements BinaryWriter-like Seek for BinaryReader.
        /// If SeekOrigin is not passed, it's using the current position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="origin"></param>
        public void Seek(long x, SeekOrigin origin = SeekOrigin.Current)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    this.BaseStream.Position = x;
                    break;

                case SeekOrigin.Current:
                    this.BaseStream.Position += x;
                    break;

                case SeekOrigin.End:
                    this.BaseStream.Position = this.BaseStream.Length - x;
                    break;
            }

            if (this.BaseStream.Position < 0 || this.BaseStream.Position > this.BaseStream.Length)
                throw new IndexOutOfRangeException("Attempted to seek beyond stream.");
        }

        /// <summary>
        /// Seek wrapper that always jump to given location from the beginning of the stream.
        /// </summary>
        /// <param name="position">Stream position to jump to.</param>
        public void Jump(long position)
        {
            Seek(position, SeekOrigin.Begin);
        }

        /// <summary>
        /// Jump overload that accepts UIntPtr as a param.
        /// </summary>
        /// <param name="pointer">Target pointer.</param>
        public void Jump(UIntPtr pointer)
        {
            Jump(pointer.ToUInt32());
        }

        #endregion

        /// <summary>
        /// Reads time-delta value from stream as found in MIDI format.
        /// </summary>
        /// <returns></returns>
        public int ReadTimeDelta()
        {
            int time = 0;
            byte next = 0;
            int ttltime = 0;

            do
            {
                byte x = ReadByte();

                time = (byte)(x & 0x7F);
                next = (byte)(x & 0x80);

                ttltime = (ttltime << 7) | time;
            }
            while (next != 0);

            return ttltime;
        }

        #region Big endian helpers

        //It probably was wrong all the time, make sure stuff still works.
        public int ReadInt32Big()
        {
            byte[] x = BitConverter.GetBytes(ReadInt32());
            Array.Reverse(x);
            return BitConverter.ToInt32(x, 0);
        }

        public uint ReadUInt32Big()
        {
            byte[] x = BitConverter.GetBytes(ReadUInt32());
            Array.Reverse(x);
            return BitConverter.ToUInt32(x, 0);
        }

        #endregion

        #region Array helpers

        public short[] ReadArrayInt16(int num)
        {
            short[] x = new short[num];

            for (int i = 0; i < num; i++)
                x[i] = ReadInt16();

            return x;
        }

        public ushort[] ReadArrayUInt16(int num)
        {
            ushort[] x = new ushort[num];

            for (int i = 0; i < num; i++)
                x[i] = ReadUInt16();

            return x;
        }

        public uint[] ReadArrayUInt32(int num)
        {
            uint[] x = new uint[num];

            for (int i = 0; i < num; i++)
                x[i] = ReadUInt32();

            return x;
        }

        #endregion

        #region List helpers

        public List<short> ReadListInt16(int num)
        {
            return ReadArrayInt16(num).ToList();
        }

        public List<uint> ReadListUInt32(int num)
        {
            return ReadArrayUInt32(num).ToList();
        }

        #endregion

        #region String helpers

        /// <summary>
        /// Reads fixed sized array of char and converts to a string.
        /// </summary>
        /// <param name="num">Number of chars to read.</param>
        /// <returns></returns>
        public string ReadStringFixed(int num)
        {
            return new string(ReadChars(num)).Split('\0')[0];
        }

        /// <summary>
        /// Reads chars 1 by 1 until 0 is met.
        /// </summary>
        /// <returns></returns>
        public string ReadStringNT()
        {
            string x = "";
            char c;

            do
            {
                c = ReadChar();
                if (c != 0) x += c;
            }
            while (c != 0);

            return x;
        }

        #endregion

        public UIntPtr ReadUIntPtr()
        {
            return (UIntPtr)ReadUInt32();
        }

        public string HexPos()
        {
            return "0x" + this.BaseStream.Position.ToString("x8");
        }
    }
}