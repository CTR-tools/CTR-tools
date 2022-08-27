using CTRFramework.Shared;
using CTRFramework.Vram;
using System.Collections.Generic;
using System.Drawing;
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

        public static IconPack FromReader(BinaryReaderEx br)
        {
            return new IconPack(br);
        }

        public void Read(BinaryReaderEx br)
        {
            int numTex = br.ReadInt32();
            int ptrTex = br.ReadInt32();
            int numGroups = br.ReadInt32();
            int ptrGroups = br.ReadInt32();

            var IconsPtrDict = new Dictionary<uint, Icon>();

            for (int i = 0; i < numTex; i++)
            {
                uint iconpos = (uint)br.Position;

                Icon icon = Icon.FromReader(br);

                if (!Icons.ContainsKey(icon.Name))
                {
                    Icons.Add(icon.Name, icon);

                    if (!IconsPtrDict.ContainsKey(iconpos))
                        IconsPtrDict.Add(iconpos, icon);
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
                Helpers.Panic(this, PanicType.Warning, "Passed null vram to IconPack.");
                return;
            }

            Helpers.CheckFolder(path);

            foreach (var icon in Icons.Values)
                icon.Save(path, tim);

            //GetFonts(tim).Save(Helpers.PathCombine(path, "largefont.png"), System.Drawing.Imaging.ImageFormat.Png);
        }

        public Bitmap GetFonts(Tim tim)
        {
            if (!Groups.ContainsKey("largefont"))
                return null;

            var glyphs = new List<Bitmap>();

            //populate glyph array
            foreach (var iconname in Groups["largefont"])
                glyphs.Add(tim.GetTexture(Icons[iconname].tl));


            Bitmap bmp = new Bitmap(160, 80);
            Graphics g = Graphics.FromImage(bmp);

            int x = 0;
            int y = 0;

            for (int i = 0; i < 49; i++)
            {
                g.DrawImage(glyphs[i], x * 16, y * 16);

                x++;
                if (x >= 10)
                {
                    x = 0;
                    y++;
                }
            }

            return bmp;
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