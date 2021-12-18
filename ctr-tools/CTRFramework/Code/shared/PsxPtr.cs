using System;
using System.Collections.Generic;

namespace CTRFramework.Shared
{
    public enum HiddenBits
    {
        None = 0,
        Bit0 = 1,
        Bit1 = 2,
        Both = 3
    }

    public class PsxPtr : IEquatable<object>, IReadWrite
    {
        public static PsxPtr Zero = new PsxPtr(0);

        public UIntPtr Address = UIntPtr.Zero;
        public HiddenBits ExtraBits = HiddenBits.None;

        private uint value => Address.ToUInt32() | (uint)ExtraBits;

        public PsxPtr(uint val)
        {
            Convert(val);
        }

        public PsxPtr(BinaryReaderEx br)
        {
            Read(br);
        }

        /// <summary>
        /// Reads the original pointer.
        /// </summary>
        /// <param name="br">BinaryReaderEx instance.</param>
        public void Read(BinaryReaderEx br)
        {
            Convert(br.ReadUInt32());
        }

        public static PsxPtr FromReader(BinaryReaderEx br)
        {
            return new PsxPtr(br);
        }

        /// <summary>
        /// Splits original value in MIPS pointer and extra hidden bits.
        /// </summary>
        /// <param name="val"></param>
        private void Convert(uint val)
        {
            Address = (UIntPtr)(val >> 2 << 2);
            ExtraBits = (HiddenBits)(val & 3);
        }

        /// <summary>
        /// Writes pointer and extra bits to the target writer.
        /// </summary>
        /// <param name="bw">BinaryWriterEx instance.</param>
        /// <param name="patchTable">Patch table to update.</param>
        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.Write((UIntPtr)value, patchTable);
        }

        public override bool Equals(object other)
        {
            if (!(other is PsxPtr))
                return false;

            return this.value == (other as PsxPtr).value;
        }

        public override int GetHashCode()
        {
            return (int)value;
        }

        public static bool operator ==(PsxPtr a, PsxPtr b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(PsxPtr a, PsxPtr b)
        {
            return !a.Equals(b);
        }

        public override string ToString()
        {
            return $"0x{Address.ToUInt32().ToString("X8")} [{ExtraBits}] <= ({value})";
        }

        public static implicit operator UIntPtr(PsxPtr me)
        {
            return me.Address;
        }

        public long GetDifference(PsxPtr other)
        {
            return Math.Abs(this.Address.ToUInt32() - other.Address.ToUInt32());
        }
    }
}