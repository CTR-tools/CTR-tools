using CTRFramework.Shared;
using CTRFramework.Vram;
using System;
using System.Drawing.Imaging;

namespace CTRFramework
{
    public class Icon : IEquatable<Icon>, IRead
    {
        public string Name { get; set; } = "default"; //16 bytes
        public int Index { get; set; } = -1;
        public TextureLayout tl { get; set; }

        public Icon()
        {
        }

        public Icon(BinaryReaderEx br) => Read(br);

        public static Icon FromReader(BinaryReaderEx br) => new Icon(br);

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
            Index = br.ReadInt32();
            tl = TextureLayout.FromReader(br);

            Helpers.Panic(this, PanicType.Debug, Name);
        }

        public void Save(string path, Tim tim)
        {
            if (tim is null)
            {
                Helpers.Panic(this, PanicType.Error, "Passed null vram.");
                return;
            }

            Helpers.CheckFolder(path);
            tim.GetTexture(tl).Save(Helpers.PathCombine(path, $"{tl.Tag};{Name}.png"), ImageFormat.Png);
        }
    }
}