using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CTRFramework
{
    class TexMap : IRead
    {
        public string name; //16 bytes
        public string group;
        public uint id;
        public TextureLayout tl;

        public TexMap()
        {
        }

        public TexMap(BinaryReader br, string g)
        {
            Read(br, g);
        }

        public void Read(BinaryReader br, string g)
        {
            group = g;
            Read(br);
        }

        public void Read(BinaryReader br)
        {
            name = System.Text.Encoding.ASCII.GetString(br.ReadBytes(16)).Split('\0')[0];
            id = br.ReadUInt32();
            tl = new TextureLayout(br);
        }
    }
}