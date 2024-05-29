using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Numerics;

namespace CTRFramework.Shared
{
    public enum VectorPadding
    {
        No = 0, Yes = 1
    }

    /// <summary>
    /// Exentend base BinaryWriter with additional helper and writing functions.
    /// </summary>
    public class BinaryWriterEx : BinaryWriter
    {
        // a shortcut for stream position, just so you don't type basestream every time.
        public long Position => BaseStream.Position;

        public BinaryWriterEx(MemoryStream ms) : base(ms)
        {
        }

        public BinaryWriterEx(FileStream ms) : base(ms)
        {
        }

        /// <summary>
        /// To be used in Save methods to avoid leftovers beyond the stream end.
        /// Happens when existing file was larger than the new one.
        /// </summary>
        public void Truncate()
        {
            BaseStream.SetLength(Position);
        }

        /// <summary>
        /// Pad to the given amount of bytes. Jumps to the closest address divisible by pad value. 
        /// For example use this when you need next value to start at an even address. pads to 4 by default.
        /// </summary>
        /// <param name="pad"></param>
        public void Pad(uint pad = 4)
        {
            if (BaseStream.Position % pad != 0)
            {
                BaseStream.Position += pad - (BaseStream.Position % pad);
            }
        }

        /// <summary>
        /// Jump to the pointer.
        /// </summary>
        /// <param name="x"></param>
        public void Jump(UIntPtr x)
        {
            Jump(x.ToUInt32());
        }

        /// <summary>
        /// Wraps seek for an absolute address jump.
        /// </summary>
        /// <param name="address"></param>
        public void Jump(long address)
        {
            Seek((int)address, SeekOrigin.Begin);
        }

        /// <summary>
        /// Hides second seek param for convenience, skips X bytes from current position.
        /// </summary>
        /// <param name="bytes"></param>
        public void Seek(int bytes)
        {
            Seek(bytes, SeekOrigin.Current);
        }

        /// <summary>
        /// Jumps to the closest 2048 aligned address.
        /// </summary>
        public void JumpNextSector()
        {
            Jump((int)((BaseStream.Position + 2047) >> 11 << 11));
        }

        /// <summary>
        /// Writes big endian integer value to the stream.
        /// </summary>
        /// <param name="value"></param>
        public void WriteBig(int value)
        {
            byte[] x = BitConverter.GetBytes(value);
            Array.Reverse(x);
            Write(x);
            //for (int i = 0; i < 4; i++) Write(x[3 - i]);
        }

        /// <summary>
        /// Writes big endian uint value to the stream
        /// </summary>
        /// <param name="value"></param>
        public void WriteBig(uint value)
        {
            byte[] x = BitConverter.GetBytes(value);
            Array.Reverse(x);
            Write(x);
            //for (int i = 0; i < 4; i++) Write(x[3 - i]);
        }

        /// <summary>
        /// Writes a pointer to the stream and adds to the patchTable if present. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="patchTable"></param>
        public void Write(UIntPtr value, List<UIntPtr> patchTable)
        {
            if (value != UIntPtr.Zero && patchTable != null)
                patchTable.Add((UIntPtr)BaseStream.Position);

            Write(value.ToUInt32());
        }

        public void WriteVector2s(Vector2 value, float scale = 1.0f)
        {
            Write((short)(Math.Round(value.X / scale)));
            Write((short)(Math.Round(value.Y / scale)));
        }

        public void WriteVector2b(Vector2 value, float scale = 1.0f)
        {
            // TODO: explain. probably should clamp or smth...
            if (value.X < 0 || value.X > 255 || value.Y < 0 || value.Y > 255)
            {
                Seek(2);
                return;
            }

            Write((byte)(Math.Round(value.X / scale)));
            Write((byte)(Math.Round(value.Y / scale)));
        }

        /// <summary>
        /// Writes vector3 to the stream in short format with padding.
        /// </summary>
        /// <param name="value">Vector</param>
        /// <param name="pad">Include padding</param>
        /// <param name="scale">Vector scale</param>
        public void WriteVector3s(Vector3 value, float scale = 1f, VectorPadding pad = VectorPadding.No)
        {
            Write((short)(Math.Round(value.X / scale)));
            Write((short)(Math.Round(value.Y / scale)));
            Write((short)(Math.Round(value.Z / scale)));

            if (pad == VectorPadding.Yes) Write((short)0);
        }

        /// <summary>
        /// Writes int in "time delta" format, used in MIDI protocol.
        /// Per byte stores 7 bits of data + 1 bit flag, denoting whether there is more data to read.
        /// </summary>
        /// <param name="value"></param>
        public void WriteTimeDelta(uint value)
        {
            var buffer = new List<byte>();

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
        }
    }
}