using CTRFramework.Shared;
using CTRFramework.Vram;
using System;

namespace CTRFramework
{
    public class Icon : IEquatable<Icon>, IRead
    {
        public string Name = "default"; //16 bytes
        public uint Index = 0;
        public TextureLayout tl;

        public Icon()
        {
        }

        public Icon(BinaryReaderEx br)
        {
            Read(br);
        }

        public static Icon FromReader(BinaryReaderEx br)
        {
            return new Icon(br);
        }

        public bool Equals(Icon other)
        {
            if (Index != other.Index)
                return false;

            if (Name != other.Name)
                return false;

            //implement texturelayout iequatable
            //if (tl != other.tl)
            //    return false;

            return true;
        }

        public void Read(BinaryReaderEx br)
        {
            Name = br.ReadStringFixed(16);
            Index = br.ReadUInt32();
            tl = TextureLayout.FromStream(br);
        }
    }
}