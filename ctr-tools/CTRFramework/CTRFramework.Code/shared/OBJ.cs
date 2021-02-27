using System;
using CTRFramework.Shared;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Globalization;

namespace CTRFramework
{
    public class OBJ
    {
        public static void FixCulture()
        {
            CultureInfo customCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = customCulture;
        }

        public static string ASCIIFace(string label, int totalv, int x, int y, int z)
        {
            return
                String.Format(
                    "{0} {1} {2} {3}\r\n",
                    label,
                    totalv + x, totalv + y, totalv + z
                    );
        }

        public static string ASCIIQuad(string label, int totalv, int totalvt)
        {
            return $"{label} {totalv + 2}/{totalvt + 2} {totalv + 1}/{totalvt + 1} {totalv + 3}/{totalvt + 3} {totalv + 4}/{totalvt + 4}";
        }

        public static string ASCIIFace(string label, int totalv, int totalvt, int x, int y, int z, float xuv, float yuv, float zuv)
        {
            return String.Format(
                "{0} {1}/{2} {3}/{4} {5}/{6}\r\n",
                label,
                totalv + x, totalvt + xuv,
                totalv + y, totalvt + yuv,
                totalv + z, totalvt + zuv
                );
        }
        

        public OBJ()
        {

        }

        public OBJ(string filename)
        {
            Read(filename);
        }


        public static OBJ FromFile(string filename)
        {
            return new OBJ(filename);
        }


        public void Read(string filename)
        {
            vertices.Clear();
            faces.Clear();

            string[] lines = File.ReadAllLines(filename);

            foreach (var line in lines)
                ParseLine(line);
        }

        string ObjectName = "empty";

        List<Vector3s> vertices = new List<Vector3s>();
        List<Vector3s> faces = new List<Vector3s>();

        public void ParseLine(string s)
        {
            if (s.Trim() == "")
            {
                Console.WriteLine("empty line");
                return;
            }

            if (s.Contains("#"))
            {
                Console.WriteLine("comment " + s);
                return;
            }

            string[] words = s.Split(' ');

            if (words.Length == 0)
                return;

            if (words[0] == "o")
            {
                ObjectName = words[1];
                Console.WriteLine("object name: " + s);
                return;
            }

            if (words[0] == "vn")
            {
                Console.WriteLine("vertex normal, skip: " + s);
                return;
            }

            if (words[0] == "vt")
            {
                Console.WriteLine("uv, skip: " + s);
                return;
            }

            if (words[0] == "v")
            {
                Console.WriteLine("it's a vertex! " + s);

                if (words.Length >= 4)
                {
                    float[] coord = new float[3];

                    for (int i = 0; i < 3; i++)
                        Single.TryParse(words[i + 1], out coord[i]);

                    vertices.Add(
                        new Vector3s(
                            (short)Math.Round(coord[0] * 100),
                            (short)Math.Round(coord[1] * 100),
                            (short)Math.Round(coord[2] * 100)
                            ));
                }

                return;
            }

            if (words[0] == "f")
            { 
                Console.WriteLine("it's a face! " + s);

                if (words.Length >= 4)
                {
                    short[] coord = new short[3];

                    for (int i = 0; i < 3; i++)
                        Int16.TryParse(words[i + 1].Split('/')[0], out coord[i]);

                    faces.Add(new Vector3s((short)(coord[0]-1), (short)(coord[1] - 1), (short)(coord[2] - 1)));
                }

                return;
            }

            if (words[0] == "s")
            {
                Console.WriteLine("smoothing group, don't need. " + s);
                return;
            }

            if (words[0] == "usemtl")
            {
                Console.WriteLine("material, don't need. " + s);
                return;
            }

            if (words[0] == "mtllib")
            {
                Console.WriteLine("material lib, don't need. " + s);
                return;
            }

            Console.WriteLine("error or unimplemented obj command " + s);
        }

        public CtrModel ConvertToCtr(short modelOffset)
        {
            CtrModel ctr = new CtrModel();
            ctr.Name = ObjectName;

            CtrHeader model = new CtrHeader();
            model.name = ObjectName + "_hi";
            model.lodDistance = -1;

            BoundingBox bb = new BoundingBox();

            foreach (var v in vertices)
            {
                if (v.X > bb.Max.X) bb.Max.X = v.X;
                if (v.Y > bb.Max.Y) bb.Max.Y = v.Y;
                if (v.Z > bb.Max.Z) bb.Max.Z = v.Z;
                if (v.X < bb.Min.X) bb.Min.X = v.X;
                if (v.Y < bb.Min.Y) bb.Min.Y = v.Y;
                if (v.Z < bb.Min.Z) bb.Min.Z = v.Z;
            }

            foreach (var v in vertices)
            {
                v.X -= bb.Min.X;
                v.Y -= bb.Min.Y;
                v.Z -= bb.Min.Z;
            }



            BoundingBox bb2 = bb.Clone();

            bb2.Min.X -= bb.Min.X;
            bb2.Min.Y -= bb.Min.Y;
            bb2.Min.Z -= bb.Min.Z;
            bb2.Max.X -= bb.Min.X;
            bb2.Max.Y -= bb.Min.Y;
            bb2.Max.Z -= bb.Min.Z;

            System.Windows.Forms.MessageBox.Show(bb.ToString() + " " + bb2.ToString());

            Console.WriteLine(bb.ToString());
            Console.WriteLine(bb2.ToString());

            model.scale = new Vector4s(
                (short)(bb2.Max.X * 10),
                (short)(bb2.Max.Y * 10),
                (short)(bb2.Max.Z * 10),
                0);

            model.vtx.Clear();

            foreach (var v in vertices)
            {
                Vector3b vv = new Vector3b(
                   (byte)((float)v.X / bb2.Max.X * 255),
                   (byte)((float)v.Z / bb2.Max.Z * 255),
                   (byte)((float)v.Y / bb2.Max.Y * 255)
                    );

                model.vtx.Add(vv);
            }



            List<short> accessed = new List<short>();
            List<Vector3b> newlist = new List<Vector3b>();

            foreach (var f in faces)
            {
                //if (f.X > 255 || f.Y > 255 || f.Z > 255)
                //    throw new Exception("too many vertices. 255 is the limit. reduce vertex count and make sure you merged all vertices.");

                CtrDraw cmd = new CtrDraw();
                cmd.texIndex = 0;
                cmd.colorIndex = 0;
                cmd.stackIndex = 87;
                Console.WriteLine(cmd.stackIndex);
                cmd.flags = CtrDrawFlags.s;

                newlist.Add(model.vtx[f.X]);
                /*
                if (accessed.Contains(cmd.stackIndex))
                {
                    cmd.flags = cmd.flags | CtrDrawFlags.v;
                }
                else
                {
                    accessed.Add(cmd.stackIndex);
                    newlist.Add(model.vtx[cmd.stackIndex]);
                }
                */


                model.drawList.Add(cmd);


                cmd = new CtrDraw();
                cmd.texIndex = 0;
                cmd.colorIndex = 1;
                cmd.stackIndex = 87;
                Console.WriteLine(cmd.stackIndex);
                cmd.flags = 0;

                newlist.Add(model.vtx[f.Z]);
                /*
                if (accessed.Contains(cmd.stackIndex))
                {
                    cmd.flags = cmd.flags | CtrDrawFlags.v;
                }
                else
                {
                    accessed.Add(cmd.stackIndex);
                    newlist.Add(model.vtx[cmd.stackIndex]);
                }
                */

                model.drawList.Add(cmd);


                cmd = new CtrDraw();
                cmd.texIndex = 0;
                cmd.colorIndex = 2;
                cmd.stackIndex = 87;
                Console.WriteLine(cmd.stackIndex);
                cmd.flags = 0;

                newlist.Add(model.vtx[f.Y]);
                /*
                if (accessed.Contains(cmd.stackIndex))
                {
                    cmd.flags = cmd.flags | CtrDrawFlags.v;
                }
                else
                {
                    accessed.Add(cmd.stackIndex);
                    newlist.Add(model.vtx[cmd.stackIndex]);
                }
                */

                model.drawList.Add(cmd);
            }

            model.vtx = newlist;

            model.posOffset = new Vector4s(
                (short)(-bb2.Max.X + 15),
                30,//(short)(bb.Min.Y + modelOffset), 
                (short)(-bb2.Max.Y), 
                0);

            model.cols.Add(new Vector4b(0xFF, 0xFF, 0xFF, 0));
            model.cols.Add(new Vector4b(0xCC, 0xCC, 0xCC, 0));
            model.cols.Add(new Vector4b(0x80, 0x80, 0x80, 0));

            ctr.Entries.Add(model);

            return ctr;
        }
    }
}