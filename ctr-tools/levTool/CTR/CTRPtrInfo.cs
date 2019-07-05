using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CTRtools
{
    class CTRPtrInfo
    {
        public int facesnum;
        public int vertexnum;

        public int ptrNgonArray;
        public uint ptrvertarray;

        public uint ptrfacearray;    //something else
        public int facenum;           //something else

        public override string ToString()
        {
            return facesnum + "\r\n" + vertexnum + "\r\n" + ptrNgonArray.ToString("X8")
                + "\r\n" + ptrvertarray.ToString("X8") + "\r\n" + ptrfacearray.ToString("X8") + "\r\n" + facesnum;
        }

        public void Read(BinaryReader br)
        {
            facesnum = br.ReadInt32();
            vertexnum = br.ReadInt32();
            br.ReadInt32();  //???
            ptrNgonArray = br.ReadInt32();
            ptrvertarray = br.ReadUInt32();
            br.ReadInt32();  //null?
            ptrfacearray = br.ReadUInt32();    //something else
            facenum = br.ReadInt32();           //something else
        }
    }
}