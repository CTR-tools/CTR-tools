using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CTRFramework.Shared;

namespace CTRFramework.Sound
{
    public class VagFrame
    {
        public byte predict_nr;
        public byte shift_factor;
        public byte flags;

        public byte[] data;

        public VagFrame()
        {
        }

        public VagFrame(BinaryReaderEx br)
        {
            Read(br);
        }

        public static VagFrame FromReader(BinaryReaderEx br)
        {
            return new VagFrame(br);
        }

        public void Read(BinaryReaderEx br)
        {
            predict_nr = br.ReadByte();
            shift_factor = (byte)(predict_nr & 0xf);
            predict_nr >>= 4;
            flags = br.ReadByte();

            data = br.ReadBytes(14);
        }
    }
}
