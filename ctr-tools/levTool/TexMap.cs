using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CTRFramework;
using System.IO;

namespace levTool
{
    class TexMap : IRead
    {
        public string name; //16 bytes
        public uint id;
        public TextureLayout tl;

        public TexMap()
        {
        }
        public TexMap(BinaryReader br)
        {
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
