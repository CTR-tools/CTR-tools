using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace model_reader
{
    class LODVertexDef
    {
        int value;

        byte flags      { get { return (byte)(value >> (8 * 3) & 0xFF); } }
        byte stackIndex { get { return (byte)(value >> (8 * 2) & 0xFF); } }
        byte colorIndex { get { return (byte)(value >> (8 * 1) & 0xFF); } }
        byte texIndex   { get { return (byte)(value >> (8 * 0) & 0xFF); } }

        public LODVertexDef(int x)
        {
            value = x;
            Console.WriteLine(x.ToString("X8") + "\t" + texIndex + "\t" + colorIndex + "\t" + stackIndex + "\t" + flags);
        }
    }
}
