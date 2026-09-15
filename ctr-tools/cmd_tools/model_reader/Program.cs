using CTRFramework;
using CTRFramework.Models;
using CTRFramework.Shared;
using CTRFramework.Vram;

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
                Meta.Version);

            if (args.Length == 0)
            {
                Console.WriteLine(
                    "{0}:\r\n\t{1}: {2}\r\n\t{3}: {4}\r\n\t{5}: {6}\r\n\t{7}: {8}\r\n",
                    "Usage",
                    "Extract level", "model_reader C:\\proto8.lev",
                    "Extract model", "model_reader C:\\crash.ctr",
                    "Convert OBJ to CTR", "model_reader C:\\crash.obj",
                    "Extract model pack", "model_reader C:\\shared.mpk"
                    );
                Console.Write("Press any key...");
                Console.ReadKey();

                return;
            }

            string filename = Path.GetFullPath(args[0]);

            Console.WriteLine($"Input file: {filename}");

            if (!(File.Exists(filename) || Directory.Exists(filename)))
            {
                Console.WriteLine("{0} doesn't exist.", filename);
                return;
            }

            if ((File.GetAttributes(filename) & FileAttributes.Directory) == FileAttributes.Directory)
            {
                foreach (var file in Directory.GetFiles(filename, "*.*", SearchOption.AllDirectories))
                {
                    Console.WriteLine(file + " " + Path.IsPathRooted(file));
                    ConvertFile(Path.IsPathRooted(file) ? file : ".\\" + file);
                }
            }
            else
            {
                filename = (Path.IsPathRooted(filename) ? "" : ".\\") + filename;
                ConvertFile(filename);
            }
        }


        static string FindVramPath(string filename)
        {
            string basepath = Path.GetDirectoryName(filename);
            string vrampath = Path.ChangeExtension(filename, "vrm");

            // in case we have not found vrm file that is called like LEVm try hardcoded paths
            if (!Helpers.IsValidPath(vrampath))
            {
                // try shared
                vrampath = Helpers.FindFirstFile(basepath, "shared.vrm");
            }

            if (!Helpers.IsValidPath(vrampath))
            {
                // try custcenes1
                vrampath = Helpers.FindFirstFile(basepath, "cutscenes1.vrm");
                if (!Helpers.IsValidPath(vrampath)) vrampath = String.Empty;
            }

            if (!Helpers.IsValidPath(vrampath))
            {
                // try custcenes2
                vrampath = Helpers.FindFirstFile(basepath, "cutscenes2.vrm");
                if (!Helpers.IsValidPath(vrampath)) vrampath = String.Empty;
            }

            if (!Helpers.IsValidPath(vrampath))
            {
                Console.WriteLine("Warning! No vram file found.\r\nPlease put shared.vrm file with mpk you want to extract.");
                vrampath = String.Empty;
            }

            return vrampath;
        }

        static void ConvertFile(string filename)
        {
            string basepath = Path.GetDirectoryName(filename);
            string name = Path.GetFileNameWithoutExtension(filename);
            string ext = Path.GetExtension(filename).ToUpper();
            string vrampath = FindVramPath(filename);

            // process extension
            switch (ext)
            {
                // level file
                case ".LEV":
                    {
                        var scene = CtrScene.FromFile(filename);
                        //scn.quads = scn.quads.OrderBy(o => o.id).ToList();
                        scene.Export(Helpers.PathCombine(basepath, name), ExportFlags.All);
                        //scene.Save(filename + "_test.lev");
                        break;
                    }

                // instanced model file
                case ".CTR":
                    {
                        var model = CtrModel.FromFile(filename);
                        model.Export(basepath, String.IsNullOrWhiteSpace(vrampath) ? null : CtrVrm.FromFile(vrampath).GetVram());

                        break;
                    }

                // OBJ 3D model file
                case ".OBJ":
                    {
                        var obj = OBJ.FromFile(filename);
                        var ctr = CtrModel.FromObj(obj);
                        ctr.Save(basepath);

                        break;
                    }

                // PLY 3D model file
                case ".PLY":
                    {
                        var ctr = CtrModel.FromPly(filename);
                        ctr.Save(basepath);

                        break;
                    }

                // model container file
                case ".MPK":
                    {
                        var mpk = ModelPack.FromFile(filename);
                        mpk.Extract(Helpers.PathCombine(basepath, name), CtrVrm.FromFile(vrampath).GetVram());

                        break;
                    }

                // unknown fle
                default:
                    Console.WriteLine($"Unsupported file: {filename}");
                    return;

            }

            Console.WriteLine("Done!");
        }
    }
}