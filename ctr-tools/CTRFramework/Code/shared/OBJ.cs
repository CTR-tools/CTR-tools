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
        public string ObjectName = "empty";

        public List<Vector3s> vertices = new List<Vector3s>();
        public List<Vector3s> distinctVerts = new List<Vector3s>();

        public List<Vector4b> colors = new List<Vector4b>();
        public List<Vector4b> distinctColors = new List<Vector4b>();

        public List<Vector3s> faces = new List<Vector3s>();
        public List<Vector3s> colinds = new List<Vector3s>();
        public List<Vector3s> vertinds = new List<Vector3s>();

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

            foreach (var c in colors)
            {
                if (!distinctColors.Contains(c))
                    distinctColors.Add(c);
            }

            foreach (var v in vertices)
            {
                if (!distinctVerts.Contains(v))
                    distinctVerts.Add(v);
            }

            Console.WriteLine($"colors: {colors.Count}");
            Console.WriteLine($"unique: {distinctColors.Count}");

            foreach (var f in faces)
            {
                if (colors.Count > 0)
                {
                    colinds.Add(new Vector3s(
                        (short)distinctColors.IndexOf(colors[f.X]),
                        (short)distinctColors.IndexOf(colors[f.Y]),
                        (short)distinctColors.IndexOf(colors[f.Z])
                        ));
                }

                if (vertices.Count > 0)
                {
                    vertinds.Add(new Vector3s(
                        (short)distinctVerts.IndexOf(vertices[f.X]),
                        (short)distinctVerts.IndexOf(vertices[f.Y]),
                        (short)distinctVerts.IndexOf(vertices[f.Z])
                        ));
                }
            }

            colors = distinctColors;
            vertices = distinctVerts;

            if (colinds.Count != faces.Count)
                Helpers.Panic(this, "face and color array length mismatch!!!");

            if (vertinds.Count != faces.Count)
                Helpers.Panic(this, "face and color array length mismatch!!!");

            if (distinctColors.Count > 127)
                Helpers.Panic(this, "Too many colors in CLUT!!!");
        }

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


                    if (words.Length >= 7)
                    {
                        float[] color = new float[3];

                        for (int i = 0; i < 3; i++)
                            Single.TryParse(words[i + 3 + 1], out color[i]);

                        Vector4b vv = new Vector4b((byte)(255 * color[0]), (byte)(255 * color[1]), (byte)(255 * color[2]), 0);
                        colors.Add(vv);
                    }
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

                    faces.Add(new Vector3s((short)(coord[0] - 1), (short)(coord[1] - 1), (short)(coord[2] - 1)));
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


    }
}