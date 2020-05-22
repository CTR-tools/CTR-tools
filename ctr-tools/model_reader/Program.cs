using CTRFramework;
using CTRFramework.Shared;
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
            //this is here to force dots in floats.
            CultureInfo customCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = customCulture;

            Console.WriteLine("CTR-Tools: model_reader\r\nDCxDemo*.\r\n");
            Console.WriteLine(Meta.GetVersion() + "\r\n");

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

        static void ConvertFile(string s)
        {
            string ext = Path.GetExtension(s);

            switch (ext)
            {
                case ".lev":
                    {
                        Scene scn = Scene.FromFile(s);
                        scn.quads = scn.quads.OrderBy(o => o.id).ToList();
                        scn.ExportAll(Path.GetDirectoryName(s), ExportFlags.All);
                        //LaunchMeshLab(objfile);

                        break;
                    }
                case ".ctr":
                    {
                        LODModel mod = new LODModel(s);

                        foreach (LODHeader lh in mod.lh)
                            Helpers.WriteToFile(".\\" + mod.name + "_" + lh.name + ".obj", lh.ToObj());

                        break;
                    }
            }
            Console.WriteLine("Done!");
        }
    }
}