using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;

namespace bash_dat
{
    class Tex
    {
        public int width;
        public int height;
        public int unk0;
        public int unk1;
        public int unk2;
        public int unk3; //possible vals = 1, 2, 8, 16
        public byte[] data;

        public Tex(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            width = br.ReadInt16();
            height = br.ReadInt16();
            unk0 = br.ReadInt32();

            if (unk1 != 0)
                Console.WriteLine("unk1 not 0!");

            unk1 = br.ReadInt32();
            unk2 = br.ReadInt32();
            unk3 = br.ReadInt32();

            if (unk3 != 1 && unk3 != 2 && unk3 != 8 && unk3 != 16)
                Console.WriteLine("unk3 not 1!" + unk3);

            data = br.ReadBytes(width * height * 2);
        }
    }

    class TexPak
    {
        public uint magic;
        public uint size;
        public short numTex;
        public short numPals;
        public uint palDataSize;

        List<byte[]> pals = new List<byte[]>();
        List<Tex> tex = new List<Tex>();

        public TexPak(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            magic = br.ReadUInt32();
            size = br.ReadUInt32();
            numTex = br.ReadInt16();
            numPals = br.ReadInt16();
            palDataSize = br.ReadUInt32();

            br.Skip(0x10);

            for (int i = 0; i < numPals; i++)
            {
                int numCols = br.ReadInt32();
                pals.Add(br.ReadBytes(numCols * 2));
            }

            for (int i = 0; i < numTex; i++)
            {
                tex.Add(new Tex(br));
            }
        }

        class Program
        {
            static void Main(string[] args)
            {

                if (args.Length > 0)
                {
                    using (BinaryReaderEx br = new BinaryReaderEx(File.OpenRead(args[0])))
                    {

                        List<uint> ptrs = br.ReadListUInt32(4);
                        /*
                        using (BinaryWriterEx bw = new BinaryWriterEx(File.Create("kek.vab")))
                        {
                            br.Jump(ptrs[1]);
                            bw.Write(br.ReadBytes((int)ptrs[2] - (int)ptrs[1]));

                            br.Jump(ptrs[0]);
                            bw.Write(br.ReadBytes((int)ptrs[1] - (int)ptrs[0]));
                        }
                        */


                        br.Jump(ptrs[0]);
                        File.WriteAllBytes("kek.vb", br.ReadBytes((int)ptrs[1] - (int)ptrs[0]));

                        br.Jump(ptrs[1]);
                        File.WriteAllBytes("kek.vh", br.ReadBytes((int)ptrs[2] - (int)ptrs[1]));

                        if (ptrs[3] != 0)
                        {
                            br.Jump(ptrs[2]);
                            File.WriteAllBytes("kek1.seq", br.ReadBytes((int)ptrs[3] - (int)ptrs[2]));

                            br.Jump(ptrs[3]);
                            File.WriteAllBytes("kek2.seq", br.ReadBytes((int)br.BaseStream.Length - (int)ptrs[2]));

                        }
                        /*
                        br.Jump(0x1ca54);

                        List<Vector3s> vv = new List<Vector3s>();

                        for (int i =0; i < 0x17e8 / 6; i++)
                        {
                            vv.Add(new Vector3s(br));
                        }

                        StringBuilder sb = new StringBuilder();

                        foreach (var v in vv)
                        {
                            sb.AppendFormat("v {0}\r\n", v.ToString(VecFormat.Numbers));
                        }
                        */
                        /*
                        br.Jump(0x3ef2);

                        List<Vector4s> vv2 = new List<Vector4s>();

                        for (int i = 0; i < 0x6a; i++)
                        {
                            vv2.Add(new Vector4s(br));
                        }

                        foreach (var v in vv2)
                        {
                            sb.AppendFormat("f {0} {1} {2}\r\n", v.X + 1, v.Y + 1, v.Z + 1);
                        }

                        File.AppendAllText("test.obj", sb.ToString());
                        */


                        /*
                        TexPak x = new TexPak(br);

                        int i = 0;
                        foreach(Tex t in x.tex)
                        {
                            BMPHeader b = new BMPHeader();
                            b.Update(t.height, t.width, 256, 4);
                            b.UpdateData(x.pals[t.unk3], t.data);

                            i++;

                            using (BinaryWriterEx bw = new BinaryWriterEx(File.OpenWrite("kek" + i + ".bmp")))
                            {
                                b.Write(bw);
                            }
                        }
                        */
                    }

                    Console.ReadKey();
                }
                else
                {


                    try
                    {
                        Directory.CreateDirectory("data");

                        //string exe = "SCUS_945.70";
                        string exe = "CRASHBSH.EXE";
                        string bsh = "CRASHBSH.DAT";

                        string exe_md5 = Helpers.CalculateMD5(exe);
                        string bsh_md5 = Helpers.CalculateMD5(bsh);

                        File.WriteAllText("md5.txt", exe_md5 + "\r\n" + bsh_md5);

                        int ptr = 0;
                        int num = 0;

                        switch (exe_md5)
                        {
                            case "ee4963398064c458e9a9b27040d639e0": ptr = 0x33784; num = 0x1E6; break; //NTSC_DEMO_SPYRO_SPLIT
                            case "f620ac01cd60c55ab0e981104f2b6c48": ptr = 0x3e910; num = 0x3E0; break; //NTSC_RELEASE
                            case "e71360b5c119f87d88acd9964ac56c21": ptr = 0x3f264; num = 0x545; break; //PAL_RELEASE
                            default: Console.WriteLine("unsupported!"); return;
                        }

                        using (BinaryReaderEx hdr = new BinaryReaderEx(File.Open(exe, FileMode.Open)))
                        {
                            using (BinaryReaderEx dat = new BinaryReaderEx(File.Open(bsh, FileMode.Open)))
                            {
                                hdr.Jump(ptr);

                                for (int i = 0; i < num; i++)
                                {
                                    int offset = hdr.ReadInt32() * 0x800;
                                    int size = hdr.ReadInt32();

                                    dat.Jump(offset);

                                    string ext = "";


                                    int check = dat.ReadInt32();

                                    switch (check)
                                    {
                                        case 0x08: ext = ".tex"; break;

                                        case 0x0C160029: ext = ".mdl"; break;
                                        case 0x09160026: ext = ".mdl2"; break;

                                        case 0x0C: ext = ".sfx1"; break;
                                        case 0x10: ext = ".sfx2"; break;
                                        case 0x14: ext = ".sfx3"; break;

                                        case 0x20000: ext = ".scrn"; break;

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
                                    File.WriteAllBytes("data\\" + i.ToString("00000") + ext, dat.ReadBytes(size));

                                    Console.Write(".");
                                }
                            }
                        }

                        Console.WriteLine("done!");
                    }
                    catch
                    {
                        Console.Write("Please put both CRASHBSH.DAT and SCUS_945.70 in this folder.");
                        Console.ReadKey();
                    }
                }
            }
        }
    }
}