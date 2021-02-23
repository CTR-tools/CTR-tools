using CTRFramework;
using CTRFramework.Shared;
using CTRFramework.Vram;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace model_reader
{
    class Program
    {
        static void Main(string[] args)
        {
            //this code should be moved to framework i guess
            //this is here to force dots in floats.
            CultureInfo customCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = customCulture;

            Console.WriteLine(
                "{0}\r\n{1}\r\n\r\n{2}\r\n",
                "CTR-Tools: model_reader by DCxDemo*.",
                "Converts LEV, DYN and MPK files to OBJ format.",
                Meta.GetVersionInfo());

            if (args.Length > 0)
            {
                if ((File.GetAttributes(args[0]) & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    foreach (string s in Directory.GetFiles(args[0], "*.lev"))
                    {
                        Console.WriteLine(s + " " + Path.IsPathRooted(s));
                        ConvertFile(Path.IsPathRooted(s) ? s : ".\\" + s);
                    }
                }
                else
                {
                    string s = args[0];
                    s = (Path.IsPathRooted(s) ? s : ".\\" + s);
                    ConvertFile(s);
                }
            }
            else
            {
                Console.WriteLine("No filename given!");
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
                        obj.ConvertToCtr().Write(basepath);

                        break;
                    }
                case ".mpk":
                    {
                        ModelPack mpk = ModelPack.FromFile(filename);
                        mpk.Extract(Path.Combine(basepath, name), CtrVrm.FromFile(Path.Combine(basepath, "shared.vrm")));

                        break;
                    }
            }

            Console.WriteLine("Done!");
        }
    }
}