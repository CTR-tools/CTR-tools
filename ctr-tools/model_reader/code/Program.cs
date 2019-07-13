using System;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
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
                            CTRVRAM vrm = null;
                            Bitmap bmp;  

                            string vrmpath = Path.ChangeExtension(args[0], ".vram");

                            if (File.Exists(vrmpath))
                            {
                                Console.WriteLine("We have VRAM!");

                                vrm = new CTRVRAM();

                                using (BinaryReader br = new BinaryReader(File.OpenRead(vrmpath)))
                                {
                                    vrm.Read(br);
                                }

                                bmp = vrm.ToBitmap();
                                bmp.Save(Path.ChangeExtension(args[0], ".png"));

                               // vrm.tim.Write("test.tim");

                                Console.WriteLine(vrm.ToString());
                            }


                            CTRModel mod = new CTRModel(args[0], format);
                            string objfile = mod.Export("obj");

                            LaunchMeshLab(objfile);

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

            Console.ReadKey();
        }


        public static void LaunchMeshLab(string objfile)
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