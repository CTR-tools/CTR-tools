using System;
using System.IO;
using System.Collections.Generic;

namespace CTRFramework.Shared
{
    public class BinaryWriterEx : BinaryWriter
    {
        public static List<UIntPtr> PointerMap = new List<UIntPtr>();

        public BinaryWriterEx(MemoryStream ms) : base(ms)
        {
        }

        public BinaryWriterEx(FileStream ms) : base(ms)
        {
        }

        public void Jump(UIntPtr x)
        {
            Jump(x.ToUInt32());
        }

        public void Jump(long x)
        {
            Seek((int)x, SeekOrigin.Begin);
        }

        public void WriteBig(int value)
        {
            byte[] x = BitConverter.GetBytes(value);
            Array.Reverse(x);
            Write(x);
            //for (int i = 0; i < 4; i++) Write(x[3 - i]);
        }

        public void WriteBig(uint value)
        {
            byte[] x = BitConverter.GetBytes(value);
            Array.Reverse(x);
            Write(x);
            //for (int i = 0; i < 4; i++) Write(x[3 - i]);
        }

        public void Write(UIntPtr value)
        {
            if (value != UIntPtr.Zero)
                PointerMap.Add((UIntPtr)BaseStream.Position);

            Write(value.ToUInt32());
        }
    }
}