using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CTRFramework
{
    class UImap
    {
        int numTex;
        int ptrTex;
        int numGroups;
        int ptrGroups;

        List<TexMap> maps = new List<TexMap>();

        public UImap(BinaryReader br)
        {
            Read(br);
        }

        public void Read(BinaryReader br)
        {
            numTex = br.ReadInt32();
            ptrTex = br.ReadInt32();
            numGroups = br.ReadInt32();
            ptrGroups = br.ReadInt32();

            br.BaseStream.Position = ptrGroups;

            List<int> gOffs = new List<int>();

            for (int i = 0; i < numGroups; i++)
                gOffs.Add(br.ReadInt32());

            foreach(int g in gOffs)
            {
                br.BaseStream.Position = g;

                string gname = System.Text.Encoding.ASCII.GetString(br.ReadBytes(16)).Split('\0')[0];
                int unk = br.ReadInt16();
                int numTex2 = br.ReadInt16();

                List<int> tOffs = new List<int>();

                for (int i = 0; i < numTex2; i++)
                    tOffs.Add(br.ReadInt32());

                foreach (int i in tOffs)
                {
                    br.BaseStream.Position = i;
                    TexMap mp = new TexMap(br, gname);
                    maps.Add(mp);
                }
            }
        }


        public void Extract(Tim tim)
        {
            foreach(TexMap tm in maps)
            {
                tim.GetTexturePage(tm.tl, String.Format("{0}{1}.png", tm.group, (tm.group + "_" + tm.name)));
            }
        }
    }
}
