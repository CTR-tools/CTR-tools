using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace CTRtools
{
    class Array1
    {
        short[] values;

        public void Read(BinaryReader br)
        {
            values = new short[6];

            for (int i =0; i < 6; i++)
            {
                values[i] = br.ReadInt16();
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (short s in values) sb.Append(s.ToString("X4") + " ");
            return sb.ToString();
        }

    }
}
