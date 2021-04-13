using CTRFramework.Shared;
using CTRFramework.Vram;
using System.Collections.Generic;
using System.IO;
using System;

namespace CTRFramework
{
    class IconPack
    {
        Dictionary<string, List<Icon>> Icons = new Dictionary<string, List<Icon>>();

        public IconPack(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            int numTex = br.ReadInt32();
            int ptrTex = br.ReadInt32();
            int numGroups = br.ReadInt32();
            int ptrGroups = br.ReadInt32();

            br.Jump(ptrGroups);

            uint[] groupPtrs = br.ReadArrayUInt32(numGroups);

            foreach (int g in groupPtrs)
            {
                br.Jump(g);

                string gname = br.ReadStringFixed(16);
                int unk = br.ReadInt16();
                int numTex2 = br.ReadInt16();

                uint[] tOffs = br.ReadArrayUInt32(numTex2);
                
                foreach (int i in tOffs)
                {
                    br.Jump(i);
                    Icon mp = new Icon(br);

                    if (!Icons.ContainsKey(gname))
                        Icons.Add(gname, new List<Icon>());

                    Icons[gname].Add(mp);
                }

                if (!Icons.ContainsKey("all"))
                    Icons.Add("all", new List<Icon>());

                br.Jump(ptrTex);

                for (int i = 0; i < numTex; i++)
                {
                    Icon mp = new Icon(br);
                    Icons["all"].Add(mp);
                }
            }
        }

        public void Extract(Tim tim, string path)
        {
            Helpers.CheckFolder(path);

            if (tim != null)
                foreach (var group in Icons.Keys)
                {
                    string subdir = Path.Combine(path, group);

                    Helpers.CheckFolder(subdir);

                    foreach (Icon tm in Icons[group])
                        tim.GetTexture(tm.tl).Save(Path.Combine(subdir, $"{tm.Name}.png"), System.Drawing.Imaging.ImageFormat.Png);
                }
        }
    }
}