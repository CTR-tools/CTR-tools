using CTRFramework.Shared;
using CTRFramework.Vram;
using CTRFramework.Bash;
using System.Collections.Generic;
using System;

namespace bash_dat
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(
                "{0}\r\n{1}\r\n\r\n{2}\r\n",
                $"CTR-tools: bash_dat - {Meta.GetSignature()}",
                "Extracts binary files from Crash Bash CRASHBSH.DAT file.",
                "Supports: TEX to BMP, SFX to VH/VB/SEQ, model to OBJ.",
                Meta.Version);

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

            foreach (var arg in args)
            {
                Helpers.Panic("Bash", PanicType.Info, $"Converting: {arg}");

                string ext = Path.GetExtension(arg).ToUpper();

                switch (ext)
                {
                    case ".TEX":
                        LoadTextureFile(arg);
                        break;
                    case ".MDL":
                        LoadModelFile(arg);
                        break;
                    case ".SFX":
                        LoadSfxFile(arg);
                        break;
                    default:
                        LoadDataFile(arg);
                        break;
                }
            }
        }

        static void LoadDataFile(string exename)
        {
            string dir = Path.GetDirectoryName(exename);
            string datname = Helpers.FindFirstFile(dir, Meta.BashDat);

            if (datname == String.Empty)
            {
                Console.WriteLine("No CRASHBSH.DAT found in this folder!");
                Console.ReadKey();
                return;
            }

            if (!File.Exists(exename) || !File.Exists(datname))
            {
                Console.WriteLine("Required files not found. Please put exe and data file in the same folder.");
                Console.ReadKey();
                return;
            }

            string exe_md5 = Helpers.CalculateMD5(exename);
            //string bsh_md5 = Helpers.CalculateMD5(datapath);

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
                var filelist = Helpers.LoadNumberedList(Meta.BashPath);

                using (var hdr = new BinaryReaderEx(File.OpenRead(exename)))
                {
                    using (var dat = new BinaryReaderEx(File.OpenRead(datname)))
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
                            int offset = hdr.ReadInt32() * Meta.SectorSize;
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

                            //need full path here for path separator fixup
                            string final_path = Helpers.PathCombine(dir, "data", (filelist.ContainsKey(i) && num == 0x3E0) ? filelist[i] : $"{i.ToString("00000")}{ext}");

                            Helpers.WriteToFile(final_path, dat.ReadBytes(size));

                            Console.Write(".");
                        }
                    }
                }

                Console.WriteLine($"\r\nDone! Extracted {num} files.");
                //Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void LoadTextureFile(string path)
        {
            var texpak = BashTexPak.FromFile(path);
            Helpers.Panic("Bash", PanicType.Info, $"textures: {texpak.numTex}, palettes: {texpak.numPals}");

            string extpath = Helpers.PathCombine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
            texpak.Export(extpath);
        }

        static void LoadModelFile(string filename)
        {
            var models = new List<BashMesh>();

            using (var br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                br.Jump(0x54);

                int numModels = br.ReadInt32();

                for (int i = 0; i < numModels; i++)
                    models.Add(new BashMesh(br));

                foreach (var m in models)
                    Helpers.Panic("LoadModelFile", PanicType.Debug, m.ToString());

                string path;

                for (int i = 0; i < numModels; i++)
                {
                    path = Helpers.PathCombine(Path.GetDirectoryName(filename), $"model_{i.ToString("00")}.obj");
                    Helpers.WriteToFile(path, models[i].ToObj());
                }

                /*
                br.Jump(0x2f0c4);

                string obj = "";

                for (int i = 0; i < 0xb20 / 8; i++)
                {
                    obj += $"v {br.ReadInt16()} {br.ReadInt16()} {br.ReadInt16()}\r\n";
                }

                path = Helpers.PathCombine(Path.GetDirectoryName(filename), $"test.obj");
                Helpers.WriteToFile(path, obj);
                */
            }
        }

        static void LoadSfxFile(string filename)
        {
            Helpers.Panic("Bash", PanicType.Info, "Loading sfx...");

            var sfx = BashSfx.FromFile(filename);
            string path = Helpers.PathCombine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename));
            sfx.Export(path);
        }
    }
}