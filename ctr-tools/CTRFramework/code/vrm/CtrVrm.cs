using CTRFramework.Shared;
using System;
using System.Drawing;

namespace CTRFramework
{
    public class CtrVrm
    {
        public static Tim FromReader(BinaryReaderEx br)
        {
            Tim buffer = new Tim(new Rectangle(0, 0, 1024, 512));
            Tim tim;

            if (br.ReadInt32() == 0x20)
            {
                for (int i = 0; i < 2; i++)
                {
                    br.ReadInt32();
                    tim = new Tim();
                    tim.Read(br);
                    buffer.DrawTim(tim);

                    Console.WriteLine(tim.ToString());
                }
            }
            else
            {
                br.BaseStream.Position = 0;
                tim = new Tim();
                tim.Read(br);
                buffer.DrawTim(tim);
            }

            return buffer;
        }
    }
}