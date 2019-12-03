using CTRFramework;
using System;
using System.Diagnostics;  //to launch meshlab
using System.Globalization; //for cultureinfo
using System.IO; //file handling
using System.Threading;

namespace modelReader
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

            if (args.Length > 0)
            {
                if ((File.GetAttributes(args[0]) & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    foreach (string s in Directory.GetFiles(args[0], "*.lev"))
                        ConvertFile(s);
                }
                else
                {
                    ConvertFile(args[0]);
                }

                /*
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
                    Console.WriteLine("Incorrect input.");
                }
                */
            }
            else
            {
                Console.WriteLine("No filename given!");
            }

            // Console.ReadKey();
        }


        static void ConvertFile(string s)
        {
            string ext = Path.GetExtension(s);

            switch (ext)
            {
                case ".lev":
                    {
                        Scene scn = new Scene(s, "obj");
                        string objfile = scn.Export("obj", Detail.Low, true);
                        objfile = scn.Export("obj", Detail.Med, false);
                        //LaunchMeshLab(objfile);

                        break;
                    }
                case ".ctr":
                    {
                        LODModel mod = new LODModel(s);
                        break;
                    }

            }
            Console.WriteLine("Done!");
        }


        static void LaunchMeshLab(string objfile)
        {
            //launch meshlab
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.Arguments = objfile;
                psi.FileName = @"C:\Program Files\VCG\MeshLab\MeshLab.exe";

                Process.Start(psi);
            }
            catch
            {
                Console.WriteLine("Install MeshLab into default path to preview models automatically.");
            }
        }
    }
}