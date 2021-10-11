using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

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

        public void Write(UIntPtr value, List<UIntPtr> patchTable)
        {
            if (value != UIntPtr.Zero && patchTable != null)
                patchTable.Add((UIntPtr)BaseStream.Position);

            Write(value.ToUInt32());
        }

        public void WriteVector3s(Vector3 value, float scale = 1.0f)
        {
            Write((short)(Math.Round(value.X / scale)));
            Write((short)(Math.Round(value.Y / scale)));
            Write((short)(Math.Round(value.Z / scale)));
        }
        public void WriteVector2s(Vector2 value, float scale = 1.0f)
        {
            Write((short)(Math.Round(value.X / scale)));
            Write((short)(Math.Round(value.Y / scale)));
        }

        public void WriteVector3sPadded(Vector3 value, float scale = 1.0f)
        {
            WriteVector3s(value, scale);
            Write((short)0);
        }
    }
}