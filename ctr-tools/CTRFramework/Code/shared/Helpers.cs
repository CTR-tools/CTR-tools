using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace CTRFramework.Shared
{
    public partial class Helpers
    {
        public static Random Random = new Random();

        public static float Normalize(float min, float max, float val)
        {
            return (val - min) / (max - min);
        }

        public static string CalculateMD5(string filename)
        {
            using (var stream = File.OpenRead(filename))
            {
                return CalculateMD5(stream);
            }
        }

        public static string CalculateMD5(Stream stream)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        //avoids excessive fragmentation
        public static void WriteToFile(string fileName, string content, System.Text.Encoding encoding = null)
        {
            if (encoding == null)
                encoding = System.Text.Encoding.Default;

            CheckFolder(fileName);

            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(fs, encoding))
                {
                    fs.SetLength(content.Length);
                    sw.Write(content);
                }
            }
        }

        public static void WriteToFile(string fileName, byte[] data)
        {
            CheckFolder(fileName);

            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                fs.SetLength(data.Length);
                fs.Write(data, 0, data.Length);
            }
        }

        public static DateTime ParseDate(string input)
        {
            DateTime result = DateTime.ParseExact(input.Replace("  ", " "), "ddd MMM d HH:mm:ss yyyy", CultureInfo.InvariantCulture);
            Helpers.Panic("Helpers.ParseDate", PanicType.Debug, result.ToString());
            return result;
        }

        /// <summary>
        /// Makes sure the target folder exists. In case filename is passed, parent path is checked.
        /// </summary>
        /// <param name="path"></param>
        public static void CheckFolder(string path)
        {
            if (Path.HasExtension(path))
                path = Path.GetDirectoryName(path);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static void BackupFile(string fileName)
        {
            string backupName = $"{fileName}.bkp";

            if (!File.Exists(backupName))
                File.Copy(fileName, backupName);
        }

        public static void RestoreFile(string fileName)
        {
            string backupName = $"{fileName}.bkp";

            if (File.Exists(backupName))
            {
                File.Delete(fileName);
                File.Move(backupName, fileName);
            }
        }

        /// <summary>
        /// Retrieves text contents as a single string.
        /// </summary>
        /// <param name="resource">Filename, typically from Meta helper class.</param>
        /// <returns></returns>
        public static string GetTextFromResource(string resource)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            using (var stream = thisAssembly.GetManifestResourceStream($"CTRFramework.Data.{resource}"))
            {
                if (stream == null)
                    return "";

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Retrieves array of lines from embedded resource.
        /// </summary>
        /// <param name="resource">Filename, typically from Meta helper class.</param>
        /// <returns></returns>
        public static string[] GetLinesFromResource(string resource)
        {
            return GetTextFromResource(resource).Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        }

        public static List<Vertex> Subdivide(List<Vertex> vertices)
        {
            if (vertices.Count != 4)
            {
                Panic("Subdivide", PanicType.Warning, "cannot subdivide if not 4 vertices");
                return null;
            }

            List<Vertex> temp = new List<Vertex>();

            for (int i = 0; i < 9; i++)
                temp.Add(new Vertex());

            temp[0] = vertices[0];
            temp[1] = vertices[1];
            temp[2] = vertices[2];
            temp[3] = vertices[3];

            temp[4] = GetAverageVertex(vertices[0], vertices[1]); // 0 and 1
            temp[5] = GetAverageVertex(vertices[0], vertices[2]); // 0 and 2

            temp[7] = GetAverageVertex(vertices[1], vertices[3]); // 1 and 3
            temp[8] = GetAverageVertex(vertices[2], vertices[3]); // 2 and 3

            temp[6] = GetAverageVertex(temp[5], temp[7]); // new 3 and 5 or new 1 and 7

            List<Vertex> result = new List<Vertex>();

            result.AddRange(new Vertex[] { temp[0], temp[4], temp[5], temp[6] });
            result.AddRange(new Vertex[] { temp[4], temp[1], temp[6], temp[7] });
            result.AddRange(new Vertex[] { temp[5], temp[6], temp[2], temp[8] });
            result.AddRange(new Vertex[] { temp[6], temp[7], temp[8], temp[3] });

            return result;
        }

        public static Vertex GetAverageVertex(Vertex a, Vertex b)
        {
            return new Vertex()
            {
                Position = (a.Position + b.Position) / 2,
                Color = new Vector4b(
                    (byte)((a.Color.X + b.Color.X) / 2),
                    (byte)((a.Color.Y + b.Color.Y) / 2),
                    (byte)((a.Color.Z + b.Color.Z) / 2),
                    (byte)((a.Color.W + b.Color.W) / 2)
                    ),
                MorphColor = new Vector4b(
                    (byte)((a.Color.X + b.Color.X) / 2),
                    (byte)((a.Color.Y + b.Color.Y) / 2),
                    (byte)((a.Color.Z + b.Color.Z) / 2),
                    (byte)((a.Color.W + b.Color.W) / 2)
                    ),
            };
        }
    }
}