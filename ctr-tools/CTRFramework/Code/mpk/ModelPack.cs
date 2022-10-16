using CTRFramework.Shared;
using CTRFramework.Vram;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CTRFramework
{
    public class ModelPack : IRead, IDisposable
    {
        public IconPack iconPack;
        public List<CtrModel> Models = new List<CtrModel>();

        public ModelPack()
        {
        }

        public ModelPack(BinaryReaderEx br) => Read(br);

        public static ModelPack FromFile(string filename)
        {
            using (var br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                return FromReader(br);
            }
        }

        public static ModelPack FromReader(BinaryReaderEx br) => new ModelPack(br);

        public void Read(BinaryReaderEx br)
        {
            int pos = (int)br.Position;

            int size = br.ReadInt32();
            br.Jump(size + 4);
            int size2 = br.ReadInt32();

            br.Jump(pos);

            //if size mismatch, assume it's demo
            if (4 + size + 4 + size2 != br.BaseStream.Length)
            {
                Helpers.Panic("ModelPack", PanicType.Assume, "Assumed Demo MPK!");
                ReadPack(br);
            }
            else
            {
                ReadPack(PatchedContainer.FromReader(br).GetReader());
            }
        }

        public void ReadPack(BinaryReaderEx br)
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

            iconPack = IconPack.FromReader(br);

            foreach (var ptr in modelPtrs)
            {
                br.Jump(ptr);

                try
                {
                    Models.Add(CtrModel.FromReader(br));
                }
                catch
                {
                    Helpers.Panic(this, PanicType.Error, $"Model loading failed: {ptr.ToString("X8")}");
                }
            }
        }

        //returns all texturelayouts found in model pack
        public Dictionary<string, TextureLayout> GetTexturesList()
        {
            var tex = new Dictionary<string, TextureLayout>();

            //loop through all icons

            foreach (var icon in iconPack.Icons.Values)
            {
                if (icon.tl == null)
                {
                    //hmm
                    Helpers.Panic(this, PanicType.Error, icon.Name);
                    continue;
                }

                tex[icon.tl.Tag] = icon.tl;
            }

            //loop through all models

            foreach (var model in Models)
                foreach (var mesh in model.Entries)
                    foreach (var tl in mesh.tl)
                    {
                        if (tl == null)
                            continue;

                        tex[tl.Tag] = tl;
                    }

            return tex;
        }

        public void Extract(string path, Tim tim)
        {
            string modelsPath = Helpers.PathCombine(path, "models");
            string texturesPath = Helpers.PathCombine(path, "textures");

            iconPack.Extract(texturesPath, tim);

            foreach (var model in Models)
            {
                model.Export(modelsPath, tim);
                model.Save(modelsPath);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Models: {Models.Count}");

            foreach (var model in Models)
                sb.AppendLine($"- {model.Name}");

            sb.AppendLine($"Textures: {iconPack.Icons.Count}");

            foreach (var icon in iconPack.Icons.Values)
                sb.AppendLine($"- {icon.Name}");

            return sb.ToString();
        }

        public void Dispose()
        {
            Models.Clear();
            iconPack = null;
        }
    }
}