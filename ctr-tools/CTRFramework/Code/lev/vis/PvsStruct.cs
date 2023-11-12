using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CTRFramework
{
    public class PvsStruct : IReadWrite
    {
        PsxPtr ptrLeafMask;
        PsxPtr ptrQuadMask;
        PsxPtr visInstSrc;
        PsxPtr visExtraSrc;

        public List<int> pvsLeafMask;
        public List<int> pvsQuadMask;

        public PvsStruct()
        {
        }

        public PvsStruct(BinaryReaderEx br) => Read(br);

        public void Read(BinaryReaderEx br)
        {
            ptrLeafMask = PsxPtr.FromReader(br);
            ptrQuadMask = PsxPtr.FromReader(br);
            visInstSrc = PsxPtr.FromReader(br);
            visExtraSrc = PsxPtr.FromReader(br);

            if (ptrLeafMask.ExtraBits != HiddenBits.None)
                Helpers.Panic(this, PanicType.Assume, "Hidden bits in visLeafSrc");

            if (ptrQuadMask.ExtraBits != HiddenBits.None)
                Helpers.Panic(this, PanicType.Assume, "Hidden bits in visFaceSrc");

            if (visInstSrc.ExtraBits != HiddenBits.None)
                Helpers.Panic(this, PanicType.Assume, "Hidden bits in visInstSrc");

            if (visExtraSrc.ExtraBits != HiddenBits.None)
                Helpers.Panic(this, PanicType.Assume, "Hidden bits in visExtraSrc");

            int pos = (int)br.Position;

            /*
            br.Jump(ptrLeafMask);

            pvsLeafMask = br.ReadArrayInt32(numNodes / 32).ToList();


            Console.Write("diff = " + (ptrQuadMask.Address - (int)br.Position ) );

            br.Jump(ptrQuadMask);

            Console.ReadKey();

            pvsQuadMask = br.ReadArrayInt32(numQuads / 32).ToList();
            */

            br.Jump(pos);
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> ptrMap = null)
        {
            ptrLeafMask.Write(bw, ptrMap);
            ptrQuadMask.Write(bw, ptrMap);
            visInstSrc.Write(bw, ptrMap);
            visExtraSrc.Write(bw, ptrMap);
        }

        public static PvsStruct FromReader(BinaryReaderEx br, int numQuads, int numLeafs) => new PvsStruct(br);
    }
}