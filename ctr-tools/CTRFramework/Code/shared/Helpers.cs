using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Numerics;

namespace CTRFramework.Shared
{
    public class Helpers
    {
        public static Random Random = new Random();

        public static string logpath = Path.Combine(Meta.BasePath, "ctrframework.log");

        public static PanicLevel panicLevel = PanicLevel.Console; //PanicLevel.File;
        public static PanicType panicType = PanicType.All ^ PanicType.Debug; //PanicType.All;

        /// <summary>
        /// Call this if something unexpected happened.
        /// </summary>
        /// <param name="sender">the object that wants to panic</param>
        /// <param name="panicType">type of panic</param>
        /// <param name="message">the message it wants to send</param>
        public static void Panic(object sender, PanicType panicType, string message)
        {
            Panic(sender.GetType().Name, panicType, message);
        }

        /// <summary>
        /// Call this if something unexpected happened.
        /// </summary>
        /// <param name="x">summary text</param>
        /// <param name="message">message text</param>
        public static void Panic(string sender, PanicType pType, string message)
        {
            if (panicLevel.HasFlag(PanicLevel.Silent))
                return;

            if (pType != PanicType.Info)
                message = $"{pType}\t{sender}:\t{message}";

            if (panicLevel.HasFlag(PanicLevel.File))
                File.AppendAllText(logpath, $"{DateTime.Now}\t{message}\r\n");

            if (panicLevel.HasFlag(PanicLevel.Console))
            {
                if (panicType.HasFlag(pType))
                {
                    Console.WriteLine(message);

                    if (panicLevel.HasFlag(PanicLevel.Pause))
                        Console.ReadKey();
                }
            }

            if (panicLevel.HasFlag(PanicLevel.Exception))
                throw new Exception(message);
        }

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

            List<Vertex> result = new List<Vertex>();

            for (int i = 0; i < 9; i++)
                result.Add(new Vertex());

            result[0] = vertices[0];
            result[1] = vertices[1];
            result[2] = vertices[2];
            result[3] = vertices[3];

            result[4] = GetAverageVertex(vertices[0], vertices[1]); // 0 and 1
            result[5] = GetAverageVertex(vertices[0], vertices[2]); // 0 and 2

            result[7] = GetAverageVertex(vertices[1], vertices[3]); // 1 and 3
            result[8] = GetAverageVertex(vertices[2], vertices[3]); // 2 and 3

            result[6] = GetAverageVertex(result[5], result[7]); // new 3 and 5 or new 1 and 7

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