using System;
using System.Collections.Generic;
using System.IO;

namespace CTRFramework.Shared
{
    public class BinaryWriterEx : BinaryWriter
    {

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

        public void Seek(int x)
        {
            Seek(x, SeekOrigin.Current);
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

        public static List<UIntPtr> PointerMap = new List<UIntPtr>();

        public void Write(UIntPtr value)
        {
            if (value != UIntPtr.Zero)
                PointerMap.Add((UIntPtr)BaseStream.Position);

            Write(value.ToUInt32());
        }
    }
}