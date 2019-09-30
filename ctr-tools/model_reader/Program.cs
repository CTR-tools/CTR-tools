using System;
using System.IO; //file handling
using System.Diagnostics;  //to launch meshlab
using System.Drawing;    //for bitmap
using System.Globalization; //for cultureinfo
using System.Threading;
using CTRFramework;

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

                string ext = Path.GetExtension(args[0]);

                switch (ext)
                {
                    case ".lev":
                        {
                            CtrVrm vrm = null;

                            string vrmpath = Path.ChangeExtension(args[0], ".vram");

                            if (File.Exists(vrmpath))
                            {
                                Console.WriteLine("We have VRAM!");

                                vrm = new CtrVrm();

                                using (BinaryReader br = new BinaryReader(File.OpenRead(vrmpath)))
                                {
                                    vrm.Read(br);
                                }

                                //vrm.buffer.ToTexturePages();
                                vrm.buffer.SaveBMP("test.bmp", BMPHeader.GrayScalePalette(16));
                                //bmp = vrm.ToBitmap();
                                //bmp.Save(Path.ChangeExtension(args[0], ".png"));

                               // vrm.tim.Write("test.tim");

                                Console.WriteLine(vrm.ToString());
                            }


                            Scene scn = new Scene(args[0], format, vrm);
                            string objfile = scn.Export("obj");

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