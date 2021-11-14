using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

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
                    if (x < 0 || x >= this.BaseStream.Length)
                        throw new IndexOutOfRangeException($"{x}, {this.BaseStream.Length}, Trying to seek out of stream range");

                    this.BaseStream.Position = x;
                    break;

                case SeekOrigin.Current:
                    if (x > this.BaseStream.Length - this.BaseStream.Position || x < -this.BaseStream.Position)
                        throw new IndexOutOfRangeException($"{x}, {this.BaseStream.Length}, Trying to seek out of stream range");

                    this.BaseStream.Position += x;
                    break;

                case SeekOrigin.End:
                    if (x < 0 || x > this.BaseStream.Position)
                        throw new IndexOutOfRangeException($"{x}, {this.BaseStream.Length}, Trying to seek out of stream range");

                    this.BaseStream.Position = this.BaseStream.Length - x;
                    break;
            }
        }

        /// <summary>
        /// Seek wrapper that always jumps to given absolute location from the beginning of the stream.
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


        public int FromTimeDelta(byte[] value)
        {
            int result = 0;

            foreach (var v in value)
            {
                result <<= 7;
                result |= v & 0x7F;
            }

            //foreach (byte v in value)
            //    Console.Write(v.ToString("X2"));

            //Console.WriteLine(" = " + result);

            return result;
        }

        /// <summary>
        /// Reads time-delta value from stream as found in MIDI format.
        /// </summary>
        /// <returns></returns>
        public int ReadTimeDelta()
        {
            byte x = 0;
            List<byte> bytes = new List<byte>();

            do
            {
                x = ReadByte();
                bytes.Add(x);
            }
            while ((x & 0x80) != 0);

            return FromTimeDelta(bytes.ToArray());

            /*
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

            return ttltime;*/
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
            byte c;
            List<byte> bytes = new List<byte>();

            do
            {
                c = ReadByte();
                if (c != 0) bytes.Add(c);
            }
            while (c != 0);

            string result = System.Text.Encoding.Default.GetString(bytes.ToArray());

            Helpers.Panic(this, PanicType.Debug, result);

            return result;
        }

        public string ReadFixedStringPtr(UIntPtr ptr, int length)
        {
            int x = (int)BaseStream.Position;
            Jump(ptr);
            string value = ReadStringFixed(16);
            Jump(x);

            return value;
        }

        #endregion

        #region Vectors

        public Vector2 ReadVector2b(float scale = 1.0f)
        {
            return new Vector2(
                ReadByte() * scale,
                ReadByte() * scale
                );
        }

        public Vector3 ReadVector3sPadded(float scale = 1.0f)
        {
            short[] values = ReadArrayInt16(4);

            return new Vector3(
                values[0] * scale,
                values[1] * scale,
                values[2] * scale
                );
        }
        public Vector3 ReadVector3s(float scale = 1.0f)
        {
            short[] values = ReadArrayInt16(3);

            return new Vector3(
                values[0] * scale,
                values[1] * scale,
                values[2] * scale
                );
        }
        public Vector4b ReadVector4b()
        {
            byte[] values = ReadBytes(4);

            return new Vector4b(
                values[0],
                values[1],
                values[2],
                values[3]
                );
        }

        public Vector2 ReadVector2s(float scale = 1.0f)
        {
            short[] values = ReadArrayInt16(2);

            return new Vector2(
                values[0] * scale,
                values[1] * scale
                );
        }

        public Vector3 ReadVector3b()
        {
            byte[] values = ReadBytes(3);

            return new Vector3(
                values[0],
                values[1],
                values[2]
                );
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