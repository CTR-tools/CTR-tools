using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CTRFramework
{
    //not a vertex.
    class LODVertex
    {
        public Vector4s coord;
        public short texU;
        public short texV;

        public LODVertex(BinaryReader br)
        {
            coord = new Vector4s(br);
            texU = br.ReadInt16();
            texV = br.ReadInt16();
        }

        public string ToOBJ()
        {
            return "v " + coord.ToString();
        }
    }
}