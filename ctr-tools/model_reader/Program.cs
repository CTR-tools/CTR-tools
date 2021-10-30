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
                Console.WriteLine(
                    "{0}:\r\n\t{1}: {2}\r\n\t{3}: {4}\r\n\t{5}: {6}\r\n\t{7}: {8}",
                    "Usage",
                    "Extract level", "model_reader C:\\proto8.lev",
                    "Extract model", "model_reader C:\\crash.ctr",
                    "Convert OBJ to CTR", "model_reader C:\\crash.obj",
                    "Extract model pack", "model_reader C:\\shared.mpk"
                    );
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
                foreach (string s in Directory.GetFiles(filename, "*.*"))
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

            string vrampath = Path.ChangeExtension(filename, "vrm");

            if (!File.Exists(vrampath))
            {
                vrampath = Path.Combine(Path.GetDirectoryName(filename), "shared.vrm");

                if (!File.Exists(vrampath))
                {
                    Console.WriteLine("Warning! No vram file found.\r\nPlease put shared.vrm file with mpk you want to extract.");
                    vrampath = "";
                }
            }

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
                        d.Export(basepath, CtrVrm.FromFile(vrampath).GetVram());

                        break;
                    }
                case ".obj":
                    {
                        OBJ obj = OBJ.FromFile(filename);
                        CtrModel ctr = CtrModel.FromObj(obj);
                        ctr.Save(basepath);

                        break;
                    }
                case ".ply":
                    {
                        CtrModel ctr = CtrModel.FromPly(filename);
                        ctr.Save(basepath);

                        break;
                    }
                case ".mpk":
                    {
                        ModelPack mpk = ModelPack.FromFile(filename);
                        mpk.Extract(Path.Combine(basepath, name), CtrVrm.FromFile(vrampath).GetVram());

                        break;
                    }
                default:
                    {
                        Console.WriteLine($"Unsupported file: {filename}");
                        return;
                    }

            }

            Console.WriteLine("Done!");
        }
    }
}