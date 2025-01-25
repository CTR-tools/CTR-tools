using System;
using System.IO;

namespace CTRFramework.Shared
{
    public class BitStreamReader : BinaryReaderEx
    {
        //we just use uint cache and track taken bits
        //once 32 bits are taken, we load new uint from the stream
        //there are no overflow checks, so be careful
        private uint cache = 0;
        private int bitsTaken = 0;

        public BitStreamReader(Stream stream) : base(stream)
        {
            stream.Position = 0;
            cache = ReadReversed();
        }
        public static BitStreamReader FromByteArray(byte[] stream)
        {
            return new BitStreamReader(new MemoryStream(stream));
        }

        public int TakeBit() => TakeBits(1);

        public int TakeBits(int amount)
        {
            if (amount == 0) return 0; //what did you expect?

            if (amount > 16)
                throw new ArgumentException("Not supposed to take more than 16 bits from this bitstream.");

            // here's what's going on with mask:
            // if bits amount = 2
            // initial 1 = 0001
            // mask = 1 << amount = 0100
            // mask - 1 = 0011

            uint mask = (uint)(1 << (amount)) - 1;

            int result = (int)(cache & mask);

            for (int i = 0; i < amount; i++)
            {
                //shift read cache
                cache >>= 1;

                //increase bits taken
                bitsTaken++;

                if (bitsTaken >= 32)
                {
                    bitsTaken = 0;
                    cache |= ReadReversed();

                    //Helpers.Panic(this, PanicType.Debug, $"cache load! cache now: {cache.ToString("X16")}");
                }
            }

            //Helpers.Panic(this, PanicType.Debug, $"{bitsTaken} bits taken, mask: {mask.ToString("X8")} value = {result}, cache before: {backup.ToString("X16")}, cache after: {cache.ToString("X16")}");

            return result;
        }


        public uint ReadReversed()
        {
            uint result = 0;
            uint value = ReadUInt32();

            //Console.WriteLine("value: " + value.ToString("X8"));

            for (int i = 0; i < 32; i++)
            {
                result |= (value & 1) << (31 - i);
                value >>= 1;
            }

            //Console.WriteLine("result: " + result.ToString("X8"));

            return result;
        }
    }
}