using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace CTRFramework.Shared
{
    public class BinaryReaderEx : BinaryReader
    {
        public long Position => BaseStream.Position;

        public bool CanRead => BaseStream.Position < BaseStream.Length;

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

        public void Pad(uint pad = 4)
        {
            if (BaseStream.Position % pad != 0)
            {
                BaseStream.Position += pad - (BaseStream.Position % pad);
            }
        }

        public void JumpNextSector()
        {
            Jump((int)((BaseStream.Position + 2047) >> 11 << 11));
        }

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
            byte[] data = ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToInt32(data, 0);
        }

        public short ReadInt16Big()
        {
            byte[] x = BitConverter.GetBytes(ReadInt16());
            Array.Reverse(x);
            return BitConverter.ToInt16(x, 0);
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

        public int[] ReadArrayInt32(int num)
        {
            int[] x = new int[num];

            for (int i = 0; i < num; i++)
                x[i] = ReadInt32();

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

        public List<int> ReadListInt32(int num)
        {
            return ReadArrayInt32(num).ToList();
        }

        public List<uint> ReadListUInt32(int num)
        {
            return ReadArrayUInt32(num).ToList();
        }

        #endregion

        #region String helpers

        /// <summary>
        /// Reads fixed sized array of char and converts to a string
        /// </summary>
        /// <param name="num">Number of chars to read.</param>
        /// <returns></returns>
        public string ReadStringFixed(int num)
        {
            return new string(ReadChars(num)).Split('\0')[0];
        }


        public static string japaneseCharset = "あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわをんぁぃぅぇぉゃゅょっ〜「」。、\u3099\u309A・?アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲンァィゥェォャュョッ?????????";

        /// <summary>
        /// Reads chars 1 by 1 until 0 is met.
        /// </summary>
        /// <returns></returns>
        public string ReadStringNT(bool forceKatakana = false)
        {
            var chars = new List<char>();
            int limit = 1024;
            char accent = '\0';

            do
            {
                byte c = ReadByte();

                if (c == 0)
                    break;

                if (c == 1)
                    accent = '\u3099';
                //chars.Add(japaneseCharset[0xBD - 0x80]);
                else if (c == 2)
                    accent = '\u309A';
                //chars.Add(japaneseCharset[0xBC - 0x80]);
                else if (c < 0x80)
                {
                    chars.Add((char)c);
                    if (accent != '\0')
                    {
                        chars.Add(accent);
                        accent = '\0';
                    }
                }
                else if (forceKatakana && c >= 0x80 && c <= 0xB7)
                {
                    chars.Add(japaneseCharset[c - 0x80 + 0x40]);
                    if (accent != '\0')
                    {
                        chars.Add(accent);
                        accent = '\0';
                    }
                }
                else
                {
                    chars.Add(japaneseCharset[c - 0x80]);
                    if (accent != '\0')
                    {
                        chars.Add(accent);
                        accent = '\0';
                    }
                }

                limit--;

                if (limit == 0)
                {
                    Helpers.Panic(this, PanicType.Warning, "string too long, maybe error?");
                    break;
                }
            }
            while (true);

            string result = new string(chars.ToArray());// System.Text.Encoding.GetEncoding(932).GetString(bytes.ToArray());

            result = result.Normalize(System.Text.NormalizationForm.FormC);

            Helpers.Panic(this, PanicType.Debug, result);

            //if (limit == 0)
            //Console.ReadKey();

            return result;
        }

        public string ReadFixedStringPtr(UIntPtr ptr, int length)
        {
            int x = (int)BaseStream.Position;
            Jump(ptr);
            string value = ReadStringFixed(length);
            Jump(x);

            return value;
        }

        #endregion

        #region Vectors

        public Vector2 ReadVector2b(float scale = 1.0f)
        {
            return new Vector2(
                (float)(ReadByte() * scale),
                (float)(ReadByte() * scale)
                );
        }

        public Vector3 ReadVector3sPadded(float scale = 1.0f, bool scaleFix = false)
        {
            if (scaleFix)
            {
                ushort[] values = ReadArrayUInt16(4);

                // a special case for labs drum. no idea why
                if ((values[1] >> 15) == 1)
                    values[1] = (ushort)((values[1] & 0x7fff) >> 2);

                return new Vector3(
                    (float)(values[0] * scale),
                    (float)(values[1] * scale),
                    (float)(values[2] * scale)
                    );
            }
            else
            {
                short[] values = ReadArrayInt16(4);

                return new Vector3(
                    (float)(values[0] * scale),
                    (float)(values[1] * scale),
                    (float)(values[2] * scale)
                    );
            }
        }

        public Vector3 ReadVector3s(float scale = 1.0f)
        {
            short[] values = ReadArrayInt16(3);

            return new Vector3(
                (float)(values[0] * scale),
                (float)(values[1] * scale),
                (float)(values[2] * scale)
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
                (float)(values[0] * scale),
                (float)(values[1] * scale)
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