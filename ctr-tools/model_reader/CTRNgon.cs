using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace model_reader
{
    class CTRNgon
    {
        public short[] ind = new short[9];
        public byte[] data;

        public CTRNgon(BinaryReader br)
        {
            for (int i = 0; i < 9; i++)
                ind[i] = br.ReadInt16();

            data = br.ReadBytes(0x2A+0x20);
        }
    }
}
