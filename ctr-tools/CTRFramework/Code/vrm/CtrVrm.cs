using CTRFramework.Shared;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace CTRFramework.Vram
{
    public class CtrVrm : IRead
    {
        public List<Tim> Tims = new List<Tim>();

        public CtrVrm()
        {
        }

        public CtrVrm(BinaryReaderEx br) => Read(br);

        public void Read(BinaryReaderEx br)
        {
            // if it starts with 0x20 magic value, it's an endless stream of tims, however, actual game is hardcoded for 2 pages max
            if (br.ReadInt32() == 0x20)
            {
                for (int i = 0; i < 2; i++)
                {
                    int pos = (int)br.BaseStream.Position;
                    int size = br.ReadInt32(); //data size
                    Tims.Add(Tim.FromReader(br));

                    if (br.BaseStream.Position - pos != size)
                        Helpers.Panic(this, PanicType.Error, "VRAM: tim size mismatch.");
                }
            }
            else //it's supposed to be just a single tim file
            {
                br.Jump(0);
                Tims.Add(Tim.FromReader(br));
            }
        }

        public static CtrVrm FromReader(BinaryReaderEx br) => new CtrVrm(br);

        public static CtrVrm FromFile(string filename)
        {
            //return empty vram if no file found
            if (!File.Exists(filename))
            {
                Helpers.Panic("CtrVram", PanicType.Warning, "Missing VRAM file, return empty.");
                return new CtrVrm();
            }

            using (var br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                return FromReader(br);
            }
        }

        public override string ToString()
        {
            string result = "";

            foreach (var tim in Tims)
                result += tim.ToString() + "\r\n";

            return result;
        }

        public static Rectangle region = new Rectangle(0, 0, 1024, 512);

        public Tim GetVram()
        {
            Tim buffer = new Tim(region, BitDepth.Bit16);

            foreach (var tim in Tims)
                buffer.DrawTim(tim);

            return buffer;
        }
    }
}