using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using ThreeDeeBear.Models.Ply;

namespace CTRFramework
{
    public class OBJ
    {
        public string ObjectName = "empty";

        public List<Vector3f> vertices = new List<Vector3f>();
        public List<Vector4b> colors = new List<Vector4b>();
        public List<int> faces = new List<int>();

        public PlyResult Result;

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
            colors.Clear();

            string[] lines = File.ReadAllLines(filename);

            foreach (var line in lines)
                ParseLine(line);

            Result = new PlyResult(vertices, faces, colors);
        }

        /// <summary>
        /// Parses single line of OBJ file.
        /// </summary>
        /// <param name="line">OBJ line.</param>
        private void ParseLine(string line)
        {
            line = line.Split('#')[0];

            if (line.Trim() == "")
            {
                Console.WriteLine("empty line");
                return;
            }

            string[] words = line.Split(' ');

            if (words.Length == 0)
                return;

            if (words[0] == "o")
            {
                ObjectName = words[1];
                Console.WriteLine("object name: " + line);
                return;
            }

            if (words[0] == "g")
            {
                Console.WriteLine("group name: " + line);
                return;
            }

            if (words[0] == "vn")
            {
                Console.WriteLine("vertex normal, skip: " + line);
                return;
            }

            if (words[0] == "vt")
            {
                Console.WriteLine("uv, skip: " + line);
                return;
            }

            if (words[0] == "v")
            {
                Console.WriteLine("it's a vertex! " + line);

                if (words.Length >= 4)
                {
                    float[] coord = new float[3];

                    for (int i = 0; i < 3; i++)
                        Single.TryParse(words[i + 1], out coord[i]);

                    vertices.Add(new Vector3f(coord[0], coord[1], coord[2]));

                    if (words.Length >= 7)
                    {
                        float[] color = new float[3];

                        for (int i = 0; i < 3; i++)
                            Single.TryParse(words[i + 3 + 1], out color[i]);

                        //assume color between 0..1 is float and multiply by 255
                        if ((color[0] > 1 || color[1] > 1 || color[2] > 1))
                        {
                            color[0] /= 255;
                            color[1] /= 255;
                            color[2] /= 255;
                        }

                        Vector4b vv = new Vector4b((byte)(255 * color[0]), (byte)(255 * color[1]), (byte)(255 * color[2]), 0);
                        colors.Add(vv);
                    }
                }

                return;
            }

            if (words[0] == "f")
            {
                Console.WriteLine("it's a face! " + line);

                if (words.Length >= 4)
                {
                    short[] coord = new short[3];

                    for (int i = 0; i < 3; i++)
                        Int16.TryParse(words[i + 1].Split('/')[0], out coord[i]);

                    faces.Add((short)(coord[0] - 1));
                    faces.Add((short)(coord[1] - 1));
                    faces.Add((short)(coord[2] - 1));
                }
                return;
            }

            if (words[0] == "s")
            {
                Console.WriteLine("smoothing group, don't need. " + line);
                return;
            }

            if (words[0] == "usemtl")
            {
                Console.WriteLine("material, don't need. " + line);
                return;
            }

            if (words[0] == "mtllib")
            {
                Console.WriteLine("material lib, don't need. " + line);
                return;
            }

            Console.WriteLine("error or unimplemented obj command " + line);
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

        static bool useNegativeIndexing = false;

        public static string ASCIIFace(string label, int totalv, int totalvt, int x, int y, int z, float xuv, float yuv, float zuv)
        {
            //OBJ format supports negative indexing to avoid large index values
            //meshlab imports wrong UVs, blender works fine
            if (useNegativeIndexing)
            {
                return $"{label} {-x}/{-xuv} {-y}/{-yuv} {-z}/{-zuv}";
            }
            else
            {
                return $"{label} {totalv + x}/{totalvt + xuv} {totalv + y}/{totalvt + yuv} {totalv + z}/{totalvt + zuv}";
            }
        }
    }
}