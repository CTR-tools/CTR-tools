using CTRFramework.Shared;
using CTRFramework.Vram;
using System;
using System.IO;

namespace vrmtool
{
    enum TimData
    {
        Name = 0,
        Bpp = 1,
        PageX = 2,
        PageY = 3,
        X = 4,
        Y = 5,
        Width = 6,
        Height = 7,
        PalX = 8,
        PalY = 9
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(
                "{0}\r\n{1}\r\n\r\n{2}\r\n",
                $"CTR-tools: vrmtool - {Meta.GetSignature()}",
                "cmd VRAM texture replacer",
                Meta.Version);

            if (args.Length == 0)
            {
                Console.WriteLine(
                    "{0}:\r\n\t{1}\r\n",
                    "Usage",
                    "vrmtool some_path\\data.vrm"
                    );
                Console.Write("Press any key...");
                Console.ReadKey();
                return;
            }

            //we expect vram file as arg, then build everything relative to it

            string _vramPath = args[0];
            string _vramName = Path.GetFileNameWithoutExtension(_vramPath);
            string _rootPath = Path.GetDirectoryName(_vramPath);
            string _newTexPath = Helpers.PathCombine(_rootPath, Meta.NewtexName);
            string _layoutsPath = Helpers.PathCombine(_rootPath, _vramName, Meta.LayoutsName);

            if (!File.Exists(_layoutsPath))
            {
                Helpers.Panic("TextureReplacer", PanicType.Warning, "_layouts.bin not found");
                return;
            }

            //create replacer and context
            var textureReplacer = new TextureReplacer()
            {
                Context = new TextureReplacerContext()
                {
                    vramPath = _vramPath,
                    newtexPath = _newTexPath,
                    dumpVram = true,
                    textures = TextureReplacer.LoadLayoutList(_layoutsPath)
                }
            };

            var result = textureReplacer.TryReplace();

            switch (result)
            {
                case TextureReplacerResult.OK: Helpers.Panic("TextureReplacer", PanicType.Info, "Replace succesful."); break;
                case TextureReplacerResult.MissingContent: Helpers.Panic("TextureReplacer", PanicType.Warning, "Not enough content provided to replacer."); break;
                case TextureReplacerResult.GeneralError: Helpers.Panic("TextureReplacer", PanicType.Error, "Replacement failed."); break;
                default: Console.Write("impossibru"); break;
            }



            /*
            string[] lines = File.ReadAllLines(args[0]);

            StringBuilder sb = new StringBuilder();
            int i = 0;

            sb.AppendLine("[TIM Files]");

            foreach (var line in lines)
            {
                if (line.Trim() == "")
                    continue;

                string[] values = line.Split(',');

                string path = Helpers.PathCombine(Path.GetDirectoryName(args[0]), values[(int)TimData.Name]);

                Bitmap bmp = (Bitmap)Bitmap.FromFile(path);

                Tim tim = new Tim(new Rectangle(
                    Int32.Parse(values[(int)TimData.PageX]) * 64 + Int32.Parse(values[(int)TimData.X]) / 4,
                    Int32.Parse(values[(int)TimData.PageY]) * 256 + Int32.Parse(values[(int)TimData.Y]),
                    bmp.Width / 4, bmp.Height), BitDepth.Bit4);

                Console.WriteLine(tim.region);

                //tim.flags = 1 << 3;

                tim.clutregion = new Rectangle(
                    Int32.Parse(values[(int)TimData.PalX]) * 16,
                    Int32.Parse(values[(int)TimData.PalY]),
                    16, 1);

                tim.LoadDataFromBitmap(path);

                string timpath = Path.ChangeExtension(path, ".tim");

                tim.Save(timpath);

                sb.AppendLine($"File_{i}={timpath}");

                i++;
            }

            sb.AppendLine("[Graphics Mode]\r\nWidth = 320\r\nHeight = 256");

            File.WriteAllText("_shared.psx", sb.ToString());

            */
        }
    }
}
