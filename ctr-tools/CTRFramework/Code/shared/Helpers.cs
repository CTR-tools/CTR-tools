using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace CTRFramework.Shared
{
    public partial class Helpers
    {
        public static Random Random = new Random();

        public static float Normalize(float min, float max, float val) => (val - min) / (max - min);

        //parses datetime format found in ctr lev files
        public static DateTime ParseDate(string input)
        {
            DateTime result = DateTime.ParseExact(input.Replace("  ", " "), "ddd MMM d HH:mm:ss yyyy", CultureInfo.InvariantCulture);
            Helpers.Panic("Helpers.ParseDate", PanicType.Debug, result.ToString());
            return result;
        }

        #region [MD5 helpers]
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
        #endregion

        #region [Filesystem helpers]
        //avoids excessive fragmentation
        public static void WriteToFile(string fileName, string content, System.Text.Encoding encoding = null)
        {
            if (encoding is null)
                encoding = System.Text.Encoding.Default;

            CheckFolder(fileName);

            using (var stream = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                using (var writer = new StreamWriter(stream, encoding))
                {
                    stream.SetLength(content.Length);
                    writer.Write(content);
                }
            }
        }

        public static void WriteToFile(string fileName, byte[] data)
        {
            CheckFolder(fileName);

            using (var stream = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                stream.SetLength(data.Length);
                stream.Write(data, 0, data.Length);
            }
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

        /// <summary>
        /// Scans folder for given filename, matches in uppercase, returns first file found or nothing
        /// I can hardly imagine someone keeping multiple dat files with different casing on linux, but it's linux
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="filename"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static string FindFirstFile(string directory, string filename, string filter = "*")
        {
            filename = filename.ToUpper();

            //btw how filter works then on linux, lol
            foreach (var file in Directory.GetFiles(directory, filter))
                if (Path.GetFileName(file).ToUpper() == filename)
                    return file;

            return String.Empty;
        }

        //replaces back and forward slashes with system specific path separator
        public static string FixPathSeparator(string directory) => directory.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);

        /// <summary>
        /// A wrapper for path combine that also fixes path separator.
        /// </summary>
        /// <param name="path">Source path</param>
        /// <returns>Fixed path</returns>
        public static string PathCombine(params string[] path) => FixPathSeparator(Path.Combine(path));

        #endregion

        #region [Backup helpers]
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
        #endregion

        #region [Resource helpers]
        public static Stream GetStreamFromZip(string resource)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            var stream = thisAssembly.GetManifestResourceStream($"CTRFramework.Data.Data.zip");

            var zip = new ZipArchive(stream, ZipArchiveMode.Read);

            foreach (var entry in zip.Entries)
                if (entry.Name == resource)
                    return entry.Open();

            return null;
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

        /// <summary>
        /// Retrieves text contents as a single string.
        /// </summary>
        /// <param name="resource">Filename, typically from Meta helper class.</param>
        /// <returns></returns>
        public static string GetTextFromResource(string resource)
        {
            //var thisAssembly = Assembly.GetExecutingAssembly();
            //using (var stream = thisAssembly.GetManifestResourceStream($"CTRFramework.Data.{resource}"))
            {
                //if (stream is null)
                //     return "";

                using (var reader = new StreamReader(GetStreamFromZip(resource)))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        //This is used for filenames mostly, loads list of "123=filename" as dictionary
        public static Dictionary<int, string> LoadNumberedList(string resource)
        {
            string[] lines = Helpers.GetLinesFromResource(resource);

            var names = new Dictionary<int, string>();

            foreach (string l in lines)
            {
                string line = l.Split('#')[0];

                if (line.Trim() != "")
                {
                    string[] bb = line.Trim().Replace(" ", "").Split('=');

                    int x = -1;
                    Int32.TryParse(bb[0], out x);

                    if (x == -1)
                    {
                        Console.WriteLine("List parsing error at: {0}", line);
                        continue;
                    }

                    if (names.ContainsKey(x))
                    {
                        Helpers.Panic("Meta", PanicType.Error, $"duplicate entry {x}");
                        continue;
                    }

                    names.Add(x, bb[1]);
                }
            }

            return names;
        }

        //This is used for samples, loads list of "XXXXXXXX=sample_name" as dictionary
        public static Dictionary<string, string> LoadTagList(string resource)
        {
            string[] lines = Helpers.GetLinesFromResource(resource);

            var names = new Dictionary<string, string>();

            foreach (string l in lines)
            {
                string line = l.Split('#')[0];

                if (line.Trim() == "") continue;

                string[] bb = line.Trim().Replace(" ", "").Split('=');

                if (names.ContainsKey(bb[0]))
                {
                    Helpers.Panic("Meta", PanicType.Error, $"duplicate entry {bb[0]}");
                    continue;
                }

                names.Add(bb[0].Trim(), bb[1].Trim());
            }

            return names;
        }
        #endregion

        #region [Subdivision helpers]
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
        #endregion
    }
}