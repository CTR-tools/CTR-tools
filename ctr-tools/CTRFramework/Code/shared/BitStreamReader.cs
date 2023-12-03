using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTRFramework.Shared
{
    public class BitStreamReader : BinaryReaderEx
    {
        //the idea is to store a long 64 value as cache
        //once we hit 32 bits taken, we load next 32 bits to the upper area
        //no overflow checks, so be careful, load more data than needed for now

        private ulong cache = 0;
        private int bitsTaken = 0;

        public BitStreamReader(Stream stream) : base(stream) 
        {
            cache = ReadReversed() | (ReadReversed() << 32);
        }

        public int ReadBits(int amount)
        {
            if (amount == 0) return 0; //what did you expect?

            if (amount > 16)
                throw new ArgumentException("Not supposed to take more than 16 bits from this bitstream.");

            //amount = 2
            //initial 1 = 0001
            //1 << amount
            //0100
            //mask - 1
            //0011

            uint mask = (uint)(1 << (amount)) - 1;

            int result = (int)(cache & mask);

            ulong backup = cache;

            for (int i = 0; i < amount; i++)
            {
                //shift read cache
                cache >>= 1;

                //increase bits taken
                bitsTaken++;

                if (bitsTaken == 32)
                {
                    bitsTaken = 0;
                    cache |= (ReadReversed() << 32);

                    Helpers.Panic(this, PanicType.Info, "cache load!");
                }
            }

            //Helpers.Panic(this, PanicType.Info, $"{bitsTaken} bits taken, mask: {mask.ToString("X8")} value = {result}, cache before: {backup.ToString("X8")}, cache after: {cache.ToString("X8")}");

            return result;
        }

        public uint ReadReversed()
        {
            uint result = 0;
            uint value = ReadUInt32();

            for (int i = 0; i < 32; i++)
            {
                result |= (value << i & 1) << (31 - i);
            }

            return result;
        }
    }
}