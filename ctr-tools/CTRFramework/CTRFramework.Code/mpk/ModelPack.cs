using CTRFramework.Shared;
using CTRFramework.Vram;
using System;
using System.Collections.Generic;
using System.IO;

namespace CTRFramework
{
    public class ModelPack : IRead
    {
        string path;
        int texOff;
        UImap map;

        public List<CtrModel> dynamics = new List<CtrModel>();

        public ModelPack()
        {
        }

        public ModelPack(string s)
        {
            path = s;

            MemoryStream ms = new MemoryStream(File.ReadAllBytes(s));
            BinaryReaderEx br = new BinaryReaderEx(ms);

            int size = br.ReadInt32();

            ms = new MemoryStream(br.ReadBytes((int)br.BaseStream.Length - 4));
            br = new BinaryReaderEx(ms);

            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            texOff = br.ReadInt32();

            uint x = 0;
            List<uint> modOffs = new List<uint>();

            do
            {
                x = br.ReadUInt32();
                if (x != 0)
                    modOffs.Add(x);
            }
            while (x != 0);


            br.Jump(texOff);

            map = new UImap(br);

            foreach (uint u in modOffs)
            {
                br.Jump(u);

                CtrModel lod = new CtrModel(br);
                dynamics.Add(lod);
            }
        }

        public void Extract(string dir, Tim tim)
        {
            map.Extract(tim, ".\\textures\\");

            foreach (CtrModel d in dynamics)
            {
                d.Export(dir);
                d.Write(dir);
            }

            Console.WriteLine("Models done!");
        }
    }
}