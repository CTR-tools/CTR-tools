using CTRFramework.Shared;
using System;

namespace CTRFramework
{
    public class WaterAnim
    {
        public PsxPtr ptrVertex;
        public PsxPtr ptrWaterAnim;
        public UInt16[] waterAnimation = new UInt16[28];

        public WaterAnim(BinaryReaderEx br)
        {
            Read(br);
        }

        public static WaterAnim FromReader(BinaryReaderEx br)
        {
            return new WaterAnim(br);
        }

        public void Read(BinaryReaderEx br)
        {
            ptrVertex = PsxPtr.FromReader(br);
            ptrWaterAnim = PsxPtr.FromReader(br);
            var currentPointer = br.BaseStream.Position;
            br.Jump(ptrWaterAnim.Address);
            for (int i = 0; i < 28; i++)
            {
                var color = br.ReadUInt16();
                /*  Based on RE the maths is:
                 *  var a = (color & 0x003f) / 63f;
                 *  var b = ((color & 0x0fc0) >> 6) / 63f;
                 *  var c = ((color & 0xf000) >> 12) / 15f;
                 * 
                 * (uVar3 & 0x3f) << 4
                 * (uVar3 & 0xfc0) >> 2
                 * (uVar3 & 0xf000) >> 4 | (uVar3 & 0xf000) >> 8
                 * Color(b,b,max(a,b),c)? can someone verify?
                 */
                waterAnimation[i] = color;
            }
            br.Jump(currentPointer);
        }
    }
}
