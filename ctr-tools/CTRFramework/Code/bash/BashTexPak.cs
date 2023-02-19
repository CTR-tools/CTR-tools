using CTRFramework.Shared;
using CTRFramework.Vram;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.IO;

namespace CTRFramework.Bash
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

        public List<BashTex> Textures = new List<BashTex>();
        public List<List<Color>> Palettes = new List<List<Color>>();

        public BashTexPak(BinaryReaderEx br) => Read(br);

        public static BashTexPak FromReader(BinaryReaderEx br) => new BashTexPak(br);

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

                Palettes.Add(palette);
            }

            for (int i = 0; i < numTex; i++)
            {
                Console.WriteLine(br.HexPos());
                Textures.Add(BashTex.FromReader(br));
            }
        }

        public void Export(string path)
        {
            Helpers.CheckFolder(path);

            int num = 0;

            foreach (var tex in Textures)
            {
                var bmp = new BMPHeader();
                bmp.Update(tex.width * 4, tex.height, 16, 4);

                byte[] pal = new byte[16 * 4];

                bool bad = false;

                int palindex = tex.unk2 / 2;

                if (palindex < 0 || palindex > Palettes.Count)
                {
                    palindex = 0;
                    bad = true;
                }

                for (int i = 0; i < 16; i++)
                {
                    pal[i * 4 + 0] = Palettes[palindex][i].B;
                    pal[i * 4 + 1] = Palettes[palindex][i].G;
                    pal[i * 4 + 2] = Palettes[palindex][i].R;
                    pal[i * 4 + 3] = Palettes[palindex][i].A;
                }

                bmp.UpdateData(pal, Tim.FixPixelOrder(tex.data));
                bmp.Save(Helpers.PathCombine(path, $"tex_{num}{(bad ? "_badpal" : "")}.bmp"));

                num++;
            }
        }
    }
}