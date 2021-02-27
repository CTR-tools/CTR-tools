using CTRFramework;
using CTRFramework.Shared;
using CTRFramework.Vram;
using System;
using System.IO;

namespace model_reader
{
    class Program
    {
        static void Main(string[] args)
        {
            OBJ.FixCulture();

            Console.WriteLine(
                "{0}\r\n{1}\r\n\r\n{2}\r\n",
                $"CTR-Tools: model_reader - {Meta.GetSignature()}",
                "Converts LEV, CTR and MPK files to OBJ format.",
                Meta.GetVersion());

            if (args.Length == 0)
            {
                Console.WriteLine("No filename given!");
                return;
            }

            string filename = args[0];

            if (!(File.Exists(filename) || Directory.Exists(filename)))
            {
                Console.WriteLine("{0} doesn't exist.", filename);
                return;
            }

            if ((File.GetAttributes(filename) & FileAttributes.Directory) == FileAttributes.Directory)
            {
                foreach (string s in Directory.GetFiles(filename, "*.lev"))
                {
                    Console.WriteLine(s + " " + Path.IsPathRooted(s));
                    ConvertFile(Path.IsPathRooted(s) ? s : ".\\" + s);
                }
            }
            else
            {
                filename = (Path.IsPathRooted(filename) ? "" : ".\\") + filename;
                ConvertFile(filename);
            }
        }


        static void ConvertFile(string filename)
        {
            string basepath = Path.GetDirectoryName(filename);
            string name = Path.GetFileNameWithoutExtension(filename);
            string ext = Path.GetExtension(filename).ToLower();

            switch (ext)
            {
                case ".lev":
                    {
                        Scene scn = Scene.FromFile(filename);
                        //scn.quads = scn.quads.OrderBy(o => o.id).ToList();
                        scn.Export(basepath, ExportFlags.All);

                        break;
                    }
                case ".ctr":
                case ".dyn":
                    {
                        CtrModel d = CtrModel.FromFile(filename);
                        d.Export(basepath);

                        break;
                    }
                case ".obj":
                    {
                        OBJ obj = OBJ.FromFile(filename);
                        obj.ConvertToCtr(100).Write(basepath);

                        break;
                    }
                case ".mpk":
                    {
                        ModelPack mpk = ModelPack.FromFile(filename);
                        mpk.Extract(Path.Combine(basepath, name), CtrVrm.FromFile(Path.Combine(basepath, "shared.vrm")));

                        break;
                    }
                default:
                    {
                        Console.WriteLine("Unsupported file.");
                        return;
                    }

            }

            Console.WriteLine("Done!");
        }
    }
}