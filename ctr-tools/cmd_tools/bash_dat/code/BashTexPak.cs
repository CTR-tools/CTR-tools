using CTRFramework.Shared;
using CTRFramework.Vram;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace bash_dat
{
    public class BashTexPak
    {
        public uint magic;
        public uint size;
        public short numTex;
        public short numPals;
        public uint skipToPal;
        public uint skipToTex;
        public uint skipToUnk;
        public uint ptrNext;
        public uint zero;

        public List<List<Color>> pals = new List<List<Color>>();
        public List<BashTex> tex = new List<BashTex>();

        public BashTexPak(BinaryReaderEx br)
        {
            Read(br);
        }

        public static BashTexPak FromReader(BinaryReaderEx br)
        {
            return new BashTexPak(br);
        }

        public static BashTexPak FromFile(string path)
        {
            using (var br = new BinaryReaderEx(File.OpenRead(path)))
            {
                return FromReader(br);
            }
        }

        public void Read(BinaryReaderEx br)
        {
            magic = br.ReadUInt32();
            size = br.ReadUInt32();
            numTex = br.ReadInt16();
            numPals = br.ReadInt16();
            skipToTex = br.ReadUInt32();
            skipToPal = br.ReadUInt32();
            skipToUnk = br.ReadUInt32();
            ptrNext = br.ReadUInt32();
            zero = br.ReadUInt32();

            for (int i = 0; i < numPals; i++)
            {
                int numCols = br.ReadInt32();

                List<Color> palette = new List<Color>();

                for (int j = 0; j < numCols; j++)
                {
                    palette.Add(Tim.Convert16(br.ReadUInt16()));
                }

                pals.Add(palette);
            }

            for (int i = 0; i < numTex; i++)
            {
                Console.WriteLine(br.HexPos());
                tex.Add(BashTex.FromReader(br));
            }
        }
    }
}