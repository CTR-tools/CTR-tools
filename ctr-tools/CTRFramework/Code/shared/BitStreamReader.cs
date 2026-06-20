using System;
using System.IO;

namespace CTRFramework.Shared
{
    public class BitStreamReader : BinaryReaderEx
    {
        // we just use uint cache and track taken bits
        // once 32 bits are taken, we load new uint from the stream
        // there are no overflow checks, so be careful

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

        /// <summary>
        /// Takes a single bit from the stream
        /// </summary>
        /// <returns></returns>
        public int TakeBit() => TakeBits(1);

        /// <summary>
        /// Get a specified amount of bits from the stream as an integer number.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public int TakeBits(int amount)
        {
            if (amount == 0) return 0; //what did you expect?

            if (amount > 16)
                throw new ArgumentException("Not supposed to take more than 16 bits from this bitstream.");

            // here's what's going on with the mask:
            // if bits amount = 2
            // initial 1 = 0001
            // mask = 1 << amount = 0100
            // mask - 1 = 0011

            // prepare mask 0x11 for 2, 0x111 for 3 etc
            uint mask = (uint)(1 << (amount)) - 1;

            // mask the cache
            int result = (int)(cache & mask);

            // for each bit taken
            for (int i = 0; i < amount; i++)
            {
                // shift cache
                cache >>= 1;

                // increase bits taken
                bitsTaken++;

                // if we've exceeded the cache, load next batch
                if (bitsTaken >= 32)
                {
                    bitsTaken = 0;
                    cache |= ReadReversed();

                    Helpers.PanicDebug(this, $"cache load! cache now: {cache.ToString("X16")}");
                }
            }

            // now it makes me wonder, does it even work properly?
            // it seems so, but can't we accidentally first take more than we should, then it fills the result with 0, and just then we load the new batch?
            // i don't think there are any obvious errors though, but it could've been like 1-3 bits difference, not so noticable.

            return result;
        }

        /// <summary>
        /// ReadUInt32, but reversed.
        /// </summary>
        /// <returns></returns>
        public uint ReadReversed()
        {
            uint result = 0;
            uint value = ReadUInt32();

            Helpers.PanicDebug(this, "value: " + value.ToString("X8"));

            for (int i = 0; i < 32; i++)
            {
                result |= (value & 1) << (31 - i);
                value >>= 1;
            }

            Helpers.PanicDebug(this, "result: " + result.ToString("X8"));

            return result;
        }
    }
}