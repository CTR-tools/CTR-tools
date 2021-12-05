using CTRFramework.Shared;
using CTRFramework.Vram;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CTRFramework
{
    public class ModelPack : IRead
    {
        string path;

        IconPack iconPack;
        public List<CtrModel> Models = new List<CtrModel>();
        public List<UIntPtr> PatchTable = new List<UIntPtr>();

        public ModelPack()
        {
        }

        public ModelPack(string s)
        {
            path = s;

            byte[] data = File.ReadAllBytes(s);

            using (var br = new BinaryReaderEx(new MemoryStream(data)))
            {
                int size = br.ReadInt32();
                br.Jump(size + 4);
                int size2 = br.ReadInt32();

                //if size mismatch, assume it's demo
                if (4 + size + 4 + size2 != br.BaseStream.Length)
                {
                    Console.WriteLine("demo!");
                    br.Jump(0);
                    Read(br);
                }
                else
                {
                    using (MemoryStream ms = new MemoryStream(data, 4, size))
                    {
                        using (var br2 = new BinaryReaderEx(ms))
                        {
                            Read(br2);
                        }
                    }

                    PatchTable.Clear();

                    br.Jump(size + 4);

                    int ptrMapSize = br.ReadInt32();

                    for (int i = 0; i < ptrMapSize / 4; i++)
                        PatchTable.Add(br.ReadUIntPtr());
                }
            }
        }

        public static ModelPack FromFile(string filename)
        {
            return new ModelPack(filename);
        }

        public void Read(BinaryReaderEx br)
        {
            int texOff = br.ReadInt32();

            //so here's what's going on, instead of reading all pointers in a loop, we assume the 1st pointer is smallest, which is also where pointermap ends.
            //we get number of models and just read the array. this will shamelessly fail, if pointers are not sorted or models don't come after pointer map.
            int firstModel = br.ReadInt32();
            int numModels = 0;

            if (firstModel != 0)
            {
                numModels = (firstModel - 4) / 4 - 1;

                br.Seek(-4); //go back
            }

            List<uint> modelPtrs = br.ReadListUInt32(numModels);

            br.Jump(texOff);
            iconPack = new IconPack(br);

            foreach (var ptr in modelPtrs)
            {
                br.Jump(ptr);

                try
                {
                    Models.Add(CtrModel.FromReader(br));
                }
                catch
                {
                    Console.WriteLine($"Model failed: {ptr.ToString("X8")}");
                }
            }

        }

        public void Extract(string path, Tim tim)
        {
            string modelsPath = Path.Combine(path, "models");
            string texturesPath = Path.Combine(path, "textures");

            iconPack.Extract(texturesPath, tim);

            foreach (var model in Models)
            {
                model.Export(modelsPath, tim);
                model.Save(modelsPath);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Models: {Models.Count}");

            foreach (var model in Models)
                sb.AppendLine($"- {model.Name}");

            sb.AppendLine($"Textures: {iconPack.Icons.Count}");

            foreach (var icon in iconPack.Icons.Values)
                sb.AppendLine($"- {icon.Name}");

            return sb.ToString();
        }
    }
}