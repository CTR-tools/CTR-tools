using System;
using System.IO;
using System.Diagnostics;

namespace model_reader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CTR-Tools: model_reader\r\nDCxDemo*.\r\n");

            if (args.Length > 0)
            {

                string format = "obj";

                try
                {
                    if (args[1] == "ply")
                        format = args[1];
                    else
                        format = "obj";
                }
                catch
                {
                    //wow
                }

                string ext = Path.GetExtension(args[0]);

                switch (ext)
                {
                    case ".lev":
                        {
                            CTRModel mod = new CTRModel(args[0], format);
                            string objfile = mod.Export("obj");

                            try
                            {
                                ProcessStartInfo psi = new ProcessStartInfo();
                                psi.Arguments = objfile;
                                psi.FileName = @"C:\Program Files\VCG\MeshLab\MeshLab.exe";

                                Process.Start(psi);
                            }
                            catch
                            {
                                Console.WriteLine("Install MeshLab into default path to view models automatically.");
                            }
                            
                            break;
                        }
                    case ".ctr":
                        {
                            LODModel mod = new LODModel(args[0]);
                            break;
                        }

                }
                Console.WriteLine("Done!");
            }
            else
            {
                Console.WriteLine("No filename given!");
            }

            var name = Console.ReadLine();
        }
    }
}