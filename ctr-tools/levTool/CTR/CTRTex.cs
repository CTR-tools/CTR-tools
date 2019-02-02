using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CTRtools
{
    class CTRTex
    {
        byte[] data;

        public CTRTex(BinaryReader br)
        {
            data = br.ReadBytes(0x14);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (byte b in data)
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}
