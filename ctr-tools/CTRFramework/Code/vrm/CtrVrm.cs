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

        public CtrVrm(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            if (br.ReadInt32() == 0x20)
            {
                for (int i = 0; i < 2; i++)
                {
                    br.ReadInt32(); //data size
                    Tims.Add(Tim.FromReader(br));
                }
            }
            else
            {
                br.Jump(0);
                Tims.Add(Tim.FromReader(br));
            }
        }

        public static CtrVrm FromReader(BinaryReaderEx br)
        {
            return new CtrVrm(br);
        }

        public static CtrVrm FromFile(string filename)
        {
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