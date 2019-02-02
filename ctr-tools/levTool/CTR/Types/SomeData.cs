using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CTRtools
{
    class SomeData
    {
        short s1;
        short s2;

        short[] data;

        public void Read(BinaryReader br)
        {
            s1 = br.ReadInt16();
            s2 = br.ReadInt16();

            data = new short[4];

            for (int i = 0; i < 4; i++)
                data[i] = br.ReadInt16();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(s1 + " " + s2 + " ");

            foreach (short s in data)
                sb.Append(s + " ");

            return sb.ToString();
        }
    }

}
