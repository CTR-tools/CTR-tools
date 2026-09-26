using CTRFramework.Shared;
using System.IO;

namespace CTRFramework.Big
{
    /// <summary>
    /// Implements extent abstraction for Bigfile.
    /// https://en.wikipedia.org/wiki/Extent_(file_systems)
    /// </summary>
    public class BigExtent
    {
        // absolute offset in bytes
        public uint Offset { get; set; } = 0;
        public uint Size { get; set; } = 0;
        public BigEntry Entry { get; set; }

        public BigExtent(uint offset, uint size)
        {
            Offset = offset;
            Size = size;
        }

        public BigExtent(BinaryReader br) => Read(br);

        public void Read(BinaryReader br)
        {
            Offset = br.ReadUInt32() * (uint)Meta.SectorSize;
            Size = br.ReadUInt32();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write((uint)(Offset / Meta.SectorSize));
            bw.Write(Size);
        }

        public void Validate(BinaryReader br)
        {
            // validate out of bounds
            if (Offset + Size > br.BaseStream.Length)
                throw new EndOfStreamException($"{this.GetType().Name}: out of bounds. {Offset + Size} vs {br.BaseStream.Length}");
        }

        public static BigExtent FromReader(BinaryReader br) => new BigExtent(br);
    }
}
