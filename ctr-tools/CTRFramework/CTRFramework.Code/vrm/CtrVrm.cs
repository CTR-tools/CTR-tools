using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace CTRFramework.Vram
{
    public class CtrVrm
    {
        public static int Width = 1024;
        public static int Height = 512;

        public static List<Rectangle> frames = new List<Rectangle>();

        private static Tim buffer = new Tim(new Rectangle(0, 0, Width, Height));

        public static Dictionary<string, Bitmap> textures = new Dictionary<string, Bitmap>();

        public static Tim FromStream(Stream str)
        {
            buffer = new Tim(new Rectangle(0, 0, Width, Height));

            using (BinaryReaderEx br = new BinaryReaderEx(str))
            {
                if (br.ReadInt32() == 0x20)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        br.ReadInt32();
                        Tim tim = new Tim(br);
                        //tim.Write("vram" + i.ToString("X2") + ".tim");
                        buffer.DrawTim(tim);

                        frames.Add(tim.region);

                        Console.WriteLine(tim.ToString());
                    }
                }
                else
                {
                    br.BaseStream.Position = 0;
                    Tim tim = new Tim(br);
                    //tim.Write("vram01.tim");
                    buffer.DrawTim(tim);

                    frames.Add(tim.region);

                    Console.WriteLine(tim.ToString());
                }

                //use this to dump whole vram as a single grayscale image
                //buffer.SaveBMP("vram.png", BMPHeader.GrayScalePalette(16));
                //buffer.Write("vram.tim");

                return buffer;
            }

        }


        public static Tim FromFile(string fn)
        {
            if (!File.Exists(fn))
                throw new FileNotFoundException($"File not found: {fn}");

            return FromStream(File.OpenRead(fn));
        }
    }
}