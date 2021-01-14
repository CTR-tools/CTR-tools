using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CTRFramework;
using CTRFramework.Vram;
using CTRFramework.Shared;

namespace vram_packer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CTR-tools, vram packer.");

            if (args.Length > 0)
            {
                Scene scn = Scene.FromFile(args[0]);

                Tim ctr = scn.ctrvram;
                ctr.SaveBMP(Path.Combine(Meta.BasePath, "test_old.png"), BMPHeader.GrayScalePalette(16));

                Dictionary<string, TextureLayout> list = scn.GetTexturesList(Detail.Med);

                foreach (string s in Directory.GetFiles(Path.Combine(Meta.BasePath, "textures"), "*.png"))
                {
                    string tag = Path.GetFileNameWithoutExtension(s);

                    Console.Write($"replacing {tag}... ");

                    if (!list.ContainsKey(tag))
                    {
                        Helpers.Panic(ctr, "missing texture entry");
                        continue;
                    }

                    Tim newtex = ctr.GetTimTexture(list[tag]);
                    newtex.LoadDataFromBitmap(s);

                    ctr.DrawTim(newtex);

                    Console.WriteLine("done.");
                }

                ctr.SaveBMP(Path.Combine(Meta.BasePath, "test_new.png"), BMPHeader.GrayScalePalette(16));

                ctr.GetTrueColorTexture(512, 0, 384, 256).Write("x01.tim");
                ctr.GetTrueColorTexture(512, 256, 512, 256).Write("x02.tim");


                //newtex = ctr.GetTimTexture(scn.GetTexturesList(Detail.Med)["0006B700"]);
                //newtex.Write(Path.Combine(Meta.BasePath, "test_after.tim"));
            }

            Console.ReadKey();
        }
    }
}
