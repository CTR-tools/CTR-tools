using CTRFramework.Shared;

namespace CTRFramework
{
    public class TexMap : IRead
    {
        public string name; //16 bytes
        public string group;
        public uint id;
        public TextureLayout tl;

        public TexMap()
        {
        }

        public TexMap(BinaryReaderEx br, string g)
        {
            Read(br, g);
        }

        public void Read(BinaryReaderEx br, string g)
        {
            group = g;
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            name = br.ReadStringFixed(16);
            id = br.ReadUInt32();
            tl = TextureLayout.FromStream(br);
        }
    }
}