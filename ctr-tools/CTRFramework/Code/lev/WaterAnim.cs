using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CTRFramework.Shared;

namespace CTRFramework
{
    public class WaterAnim
    {
        public PsxPtr ptrVertex;
        public PsxPtr ptrWaterAnim;

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
        }
    }
}
