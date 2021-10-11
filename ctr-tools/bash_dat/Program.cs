using CTRFramework.Shared;
using CTRFramework.Vram;
using System;
using System.Collections.Generic;
using System.IO;

namespace bash_dat
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(
                "{0}\r\n{1}\r\n\r\n{2}\r\n",
                $"CTR-tools: bash_dat - {Meta.GetSignature()}",
                "Extracts binary files from Crash Bash CRASHBSH.DAT file and converts TEX to BMP.",
                Meta.GetVersion());

            if (args.Length == 0)
            {
                Console.WriteLine(
                    "{0}:\r\n\t{1}: {2}\r\n\t{3}: {4}",
                    "Usage",
                    "Extract files example", "bash_dat C:\\path_to.exe",
                    "Extract textures example", "bash_dat C:\\some_dir\\some_file.tex"
                    );

                return;
            }

            string ext = Path.GetExtension(args[0]).ToLower();

            switch (ext)
            {
                case ".tex":
                    LoadTextureFile(args[0]);
                    break;
                case ".mdl":
                    LoadModelFile(args[0]);
                    break;
                default:
                    LoadDataFile(args[0]);
                    break;
            }
        }

        static void LoadDataFile(string filename)
        {
            string dir = Path.GetDirectoryName(filename);
            string datapath = Path.Combine(dir, "CRASHBSH.DAT");

            if (!File.Exists(filename) || !File.Exists(datapath))
            {
                Console.WriteLine("Required files not found. Please put exe and data file in the same folder.");
                Console.ReadKey();
                return;
            }

            string exe_md5 = Helpers.CalculateMD5(filename);
            string bsh_md5 = Helpers.CalculateMD5(datapath);

            //File.WriteAllText("md5.txt", exe_md5 + "\r\n" + bsh_md5);

            int ptr = 0;
            int num = 0;
            string version = "Unknown";

            switch (exe_md5)
            {
                case "a45627fa6c3d1768f8ad56fb46569f06": ptr = 0x335A0; num = 0x1CA; version = "OPSM 38 Demo"; break;
                case "ee4963398064c458e9a9b27040d639e0": ptr = 0x33784; num = 0x1E6; version = "Spyro Split / Winter Jampack 2000 / demo disc 1.3"; break;
                case "98e02493600b898bcacbdcb129e9019f": ptr = 0x3483C; num = 0x241; version = "Spyro 3 NTSC Demo"; break;
                case "0f35ba94f0ce49b0e6fe8e2012e5f1b4": ptr = 0x34F40; num = 0x276; version = "Spyro 3 PAL Demo"; break;
                case "db497990f79454bcbd41a770df3692ef": ptr = 0x35238; num = 0x276; version = "Euro Demo xx?"; break;
                case "b9a576bc33399addb51779c1e2e2bc42": ptr = 0x35850; num = 0x2A6; version = "Sep 14, preview prototype"; break;
                case "e830b0f1b91c1edeef42a60d3160b752": ptr = 0x3E97c; num = 0x3D2; version = "JAP Trial"; break;
                case "f620ac01cd60c55ab0e981104f2b6c48": ptr = 0x3E910; num = 0x3E0; version = "NTSC Release"; break;
                case "ce7e3fe1bf226cc8dd195e025725fdd1": ptr = 0x3F988; num = 0x4D9; version = "Oct 9 PAL prototype"; break;
                case "e9ad2756fe43d11e3d93af05acafef71": ptr = 0x3EE30; num = 0x4DD; version = "JAP Release"; break;
                case "e71360b5c119f87d88acd9964ac56c21": ptr = 0x3F264; num = 0x545; version = "PAL Release"; break;

                default: Console.WriteLine($"Unknown file: {exe_md5}"); return;
            }

            Console.WriteLine(version);

            try
            {
                Dictionary<int, string> filelist = Meta.LoadNumberedList("bash_filelist.txt");

                using (BinaryReaderEx hdr = new BinaryReaderEx(File.Open(filename, FileMode.Open)))
                {
                    using (BinaryReaderEx dat = new BinaryReaderEx(File.Open(datapath, FileMode.Open)))
                    {
                        string magic = hdr.ReadStringFixed(8);

                        if (magic != "PS-X EXE")
                        {
                            Console.WriteLine("Not a PS-X EXE.");
                            return;
                        }

                        hdr.Jump(ptr);

                        for (int i = 0; i < num; i++)
                        {
                            int offset = hdr.ReadInt32() * 0x800;
                            int size = hdr.ReadInt32();

                            dat.Jump(offset);

                            string ext = ".bin";
                            int check = dat.ReadInt32();

                            switch (check)
                            {
                                case 0x08: ext = ".tex"; break;

                                case 0x0C160029: ext = ".mdl"; break;
                                case 0x09160026: ext = ".mdl2"; break;

                                case 0x0C: ext = ".sfx1"; break;
                                case 0x10: ext = ".sfx2"; break;
                                case 0x14: ext = ".sfx3"; break;

                                case 0x20000: ext = ".tga"; break;

                                case 0x7:
                                case 0x9:
                                case 0xA:
                                case 0xB:
                                case 0xD:
                                case 0xE:
                                case 0xF:
                                case 0x12:
                                case 0x11:
                                    ext = ".map"; break;

                                case 0x5D:
                                case 0x5E:
                                case 0x5F:
                                case 0x60:
                                case 0x61:
                                case 0x62:
                                case 0x63:
                                case 0x64:
                                case 0x65:
                                case 0x66:
                                case 0x67:
                                case 0x68:
                                case 0x69:
                                case 0x6A:
                                case 0x6B:
                                    ext = ".code"; break;
                            }

                            dat.Jump(offset);

                            string final_path = Path.Combine(dir, "data", (filelist.ContainsKey(i) && num == 0x3E0) ? filelist[i] : $"{i.ToString("00000")}{ext}");

                            Helpers.WriteToFile(final_path, dat.ReadBytes(size));

                            Console.Write(".");
                        }
                    }
                }

                Console.WriteLine($"\r\nDone! Extracted {num} files.");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void LoadTextureFile(string filename)
        {
            using (BinaryReaderEx br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                TexPak x = new TexPak(br);

                Console.WriteLine(x.numPals + " " + x.numTex);

                string path = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename));

                Helpers.CheckFolder(path);

                int num = 0;

                foreach (Tex t in x.tex)
                {
                    BMPHeader b = new BMPHeader();
                    b.Update(t.width * 4, t.height, 16, 4);

                    byte[] pal = new byte[16 * 4];

                    bool bad = false;

                    int palindex = t.unk2 / 2;

                    if (palindex < 0 || palindex > x.pals.Count)
                    {
                        palindex = 0;
                        bad = true;
                    }

                    for (int i = 0; i < 16; i++)
                    {
                        pal[i * 4 + 0] = x.pals[palindex][i].B;
                        pal[i * 4 + 1] = x.pals[palindex][i].G;
                        pal[i * 4 + 2] = x.pals[palindex][i].R;
                        pal[i * 4 + 3] = x.pals[palindex][i].A;
                    }


                    b.UpdateData(pal, Tim.FixPixelOrder(t.data));

                    using (BinaryWriterEx bw = new BinaryWriterEx(File.OpenWrite($"{path}\\tex_" + num + (bad ? "_badpal" : "") + ".bmp")))
                    {
                        b.Write(bw);
                    }

                    num++;
                }
            }
        }

        static void LoadModelFile(string filename)
        {
            List<BashMesh> models = new List<BashMesh>();

            using (BinaryReaderEx br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                br.Jump(0x54);

                int numModels = br.ReadInt32();

                for (int i = 0; i < numModels; i++)
                    models.Add(new BashMesh(br));

                foreach (var m in models)
                    Console.WriteLine(m.ToString());

                for (int i = 0; i < numModels; i++)
                    Helpers.WriteToFile(Path.Combine(Path.GetDirectoryName(filename), $"model_{i.ToString("00")}.obj"), models[i].ToObj());
            }
        }
    }
}