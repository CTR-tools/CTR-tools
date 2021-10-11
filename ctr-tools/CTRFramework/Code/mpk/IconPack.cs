using CTRFramework.Shared;
using CTRFramework.Vram;
using System.Collections.Generic;
using System.Text;

namespace CTRFramework
{
    public class IconPack : IRead
    {
        public Dictionary<string, Icon> Icons = new Dictionary<string, Icon>();
        public Dictionary<string, List<string>> Groups = new Dictionary<string, List<string>>();

        public IconPack()
        {
        }

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

            Dictionary<uint, Icon> IconsPtrDict = new Dictionary<uint, Icon>();

            for (int i = 0; i < numTex; i++)
            {
                Icon icon = Icon.FromReader(br);

                if (!Icons.ContainsKey(icon.Name))
                {
                    Icons.Add(icon.Name, icon);

                    if (!IconsPtrDict.ContainsKey((uint)br.BaseStream.Position))
                        IconsPtrDict.Add((uint)br.BaseStream.Position, icon);
                }
            }

            if (br.ReadInt32() != 0)
                Helpers.Panic(this, PanicType.Assume, $"dummy != 0");

            br.Jump(ptrGroups);

            uint[] groupPtrs = br.ReadArrayUInt32(numGroups);

            for (int i = 0; i < numGroups; i++)
            {
                string gname = br.ReadStringFixed(16);
                int unk = br.ReadInt16();
                int numTex2 = br.ReadInt16();

                if (!Groups.ContainsKey(gname))
                    Groups.Add(gname, new List<string>());

                uint[] tOffs = br.ReadArrayUInt32(numTex2);

                foreach (var ptr in tOffs)
                    if (IconsPtrDict.ContainsKey(ptr))
                        Groups[gname].Add(IconsPtrDict[ptr].Name);
            }
        }

        public void Extract(string path, Tim tim)
        {
            if (tim == null)
            {
                Helpers.Panic(this, PanicType.Error, "Passed null vram.");
                return;
            }

            Helpers.CheckFolder(path);

            foreach (var icon in Icons.Values)
                icon.Save(path, tim);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var icon in Icons.Values)
                sb.AppendLine(icon.Name);

            return sb.ToString();
        }
    }
}