using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace CTRFramework
{
    public class ModelPack : IRead
    {
        string path;
        int texOff;

        List<uint> modOffs = new List<uint>();
        UImap map;

        public List<LODModel> lodmods = new List<LODModel>();

        public ModelPack()
        {
        }

        public ModelPack(string s)
        {
            path = s;

            MemoryStream ms = new MemoryStream(File.ReadAllBytes(s));
            BinaryReader br = new BinaryReader(ms);

            int size = br.ReadInt32();

            ms = new MemoryStream(br.ReadBytes((int)br.BaseStream.Length - 4));
            br = new BinaryReader(ms);

            Read(br);
        }

        public void Read(BinaryReader br)
        {
            texOff = br.ReadInt32();

            uint x = 0;

            do
            {
                x = br.ReadUInt32();
                if (x != 0)
                    modOffs.Add(x);
            }
            while (x != 0);


            br.BaseStream.Position = texOff;

            map = new UImap(br);


            foreach (uint u in modOffs)
            {
                br.BaseStream.Position = u;

                LODModel lod = new LODModel(br);
                lodmods.Add(lod);
            }
        }

        public void Extract(Tim tim)
        {
            map.Extract(tim);
        }
    }
}