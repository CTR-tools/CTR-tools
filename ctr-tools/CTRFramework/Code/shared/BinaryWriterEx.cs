using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace CTRFramework.Shared
{
    public class BinaryWriterEx : BinaryWriter
    {
        public long Position => BaseStream.Position;

        public BinaryWriterEx(MemoryStream ms) : base(ms)
        {
        }

        public BinaryWriterEx(FileStream ms) : base(ms)
        {
        }

        public void Pad(uint pad = 4)
        {
            if (BaseStream.Position % pad != 0)
            {
                BaseStream.Position += pad - (BaseStream.Position % pad);
            }
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

        public void JumpNextSector()
        {
            Jump((int)((BaseStream.Position + 2047) >> 11 << 11));
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
        public void WriteVector2b(Vector2 value, float scale = 1.0f)
        {
            if (value.X < 0 || value.X > 255 || value.Y < 0 || value.Y > 255)
            {
                Seek(2);
                return;
            }

            Write((byte)(Math.Round(value.X / scale)));
            Write((byte)(Math.Round(value.Y / scale)));
        }

        public void WriteVector3sPadded(Vector3 value, float scale = 1.0f)
        {
            WriteVector3s(value, scale);
            Write((short)0);
        }

        public void WriteTimeDelta(uint value)
        {
            List<byte> buffer = new List<byte>();

            do
            {
                buffer.Add((byte)(value & 0x7F));
                value >>= 7;
            }
            while (value > 0);

            buffer.Reverse();

            for (int i = 0; i < buffer.Count - 1; i++)
                buffer[i] |= 0x80;

            Write(buffer.ToArray());

            /*
            int buffer = value & 0x7F;

            while (value != (value >> 7))
            {
                value = value >> 7;
                buffer <<= 8;
                buffer |= ((value & 0x7F) | 0x80);
            }

            while (true)
            {
                Write((byte)buffer);
                buffer >>= 8;
                if ((buffer & 0x80) == 0)
                    break;

            }
            */
        }
    }
}