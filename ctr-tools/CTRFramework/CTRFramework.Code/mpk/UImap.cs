using CTRFramework.Shared;
using CTRFramework.Vram;
using System.Collections.Generic;

namespace CTRFramework
{
    class UImap
    {
        int numTex;
        int ptrTex;
        int numGroups;
        int ptrGroups;

        List<TexMap> maps = new List<TexMap>();

        public UImap(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            numTex = br.ReadInt32();
            ptrTex = br.ReadInt32();
            numGroups = br.ReadInt32();
            ptrGroups = br.ReadInt32();

            br.BaseStream.Position = ptrGroups;

            uint[] gOffs = br.ReadArrayUInt32(numGroups);

            foreach (int g in gOffs)
            {
                br.Jump(g);

                string gname = br.ReadStringFixed(16);
                int unk = br.ReadInt16();
                int numTex2 = br.ReadInt16();

                uint[] tOffs = br.ReadArrayUInt32(numTex2);
                /*
                foreach (int i in tOffs)
                {
                    br.Jump(i);
                    TexMap mp = new TexMap(br, gname);
                    maps.Add(mp);
                }
                */

                br.BaseStream.Position = ptrTex;

                for (int i = 0; i < numTex; i++)
                {
                    TexMap mp = new TexMap(br, "none");
                    maps.Add(mp);
                }
            }
        }


        public void Extract(Tim tim)
        {
            if (tim != null)
                foreach (TexMap tm in maps)
                    tim.GetTexture(tm.tl, "textures", tm.name);
        }
    }
}