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

        public void Skip(long x)
        {
            this.BaseStream.Position += x;
        }

        public void Jump(long x)
        {
            this.BaseStream.Position = x;
        }

        public static BinaryReaderEx FromFile(string s)
        {
            byte[] data = File.ReadAllBytes(s);
            MemoryStream ms = new MemoryStream(data);

            return new BinaryReaderEx(ms);
        }

        public string HexPos()
        {
            return "0x" + this.BaseStream.Position.ToString("x8");
        }

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

        public int ReadInt32Big()
        {
            byte[] x = BitConverter.GetBytes(ReadInt32());
            x.Reverse();
            return BitConverter.ToInt32(x, 0);
        }
        public uint ReadUInt32Big()
        {
            byte[] x = BitConverter.GetBytes(ReadUInt32());
            x.Reverse();
            return BitConverter.ToUInt32(x, 0);
        }

        public short[] ReadArrayInt16(int num)
        {
            short[] kek = new short[num];

            for (int i = 0; i < num; i++)
                kek[i] = ReadInt16();

            return kek;
        }

        public ushort[] ReadArrayUInt16(int num)
        {
            ushort[] kek = new ushort[num];

            for (int i = 0; i < num; i++)
                kek[i] = ReadUInt16();

            return kek;
        }

        public uint[] ReadArrayUInt32(int num)
        {
            uint[] kek = new uint[num];

            for (int i = 0; i < num; i++)
                kek[i] = ReadUInt32();

            return kek;
        }

        public List<short> ReadListInt16(int num)
        {
            List<short> buf = new List<short>();
            buf.AddRange(ReadArrayInt16(num));
            return buf;
        }

        public List<uint> ReadListUInt32(int num)
        {
            List<uint> buf = new List<uint>();
            buf.AddRange(ReadArrayUInt32(num));
            return buf;
        }

        public string ReadStringFixed(int num)
        {
            return new string(ReadChars(num)).Split('\0')[0];
        }

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
    }
}
