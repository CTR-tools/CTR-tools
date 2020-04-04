using CTRFramework.Shared;
using System;
using System.Drawing;

namespace CTRFramework.Vram
{
    public class CtrVrm
    {
        public static int Width = 1024;
        public static int Height = 512;

        public static Tim FromReader(BinaryReaderEx br)
        {
            Tim buffer = new Tim(new Rectangle(0, 0, Width, Height));

            if (br.ReadInt32() == 0x20)
            {
                for (int i = 0; i < 2; i++)
                {
                    br.ReadInt32();
                    Tim tim = new Tim(br);
                    //tim.Write("vram" + i.ToString("X2") + ".tim");
                    buffer.DrawTim(tim);

                    Console.WriteLine(tim.ToString());
                }
            }
            else
            {
                br.BaseStream.Position = 0;
                Tim tim = new Tim(br);
                //tim.Write("vram01.tim");
                buffer.DrawTim(tim);
            }

            GC.Collect();

            return buffer;
        }
    }
}