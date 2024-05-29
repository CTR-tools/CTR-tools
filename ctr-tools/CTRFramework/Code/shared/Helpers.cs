using CTRFramework.Vram;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Numerics;
using System.Reflection;
using System.Xml;

namespace CTRFramework.Shared
{
    public partial class Helpers
    {
        public static Random Random = new Random();

        // so these are hardcoded fixed point math fractional bits to restore the float value
        public static readonly float GteScaleLarge = 1.0f / (1 << 12); //4096 = 1.0
        public static readonly float GteScaleSmall = 1.0f / (1 << 8);  //256 = 1.0

        // Math.Clump since .NET 6
        public static float Normalize(float min, float max, float val) => (val - min) / (max - min);

        // parses datetime format found in ctr lev files
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
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
        #endregion

        #region [Filesystem helpers]
        
        /// <summary>
        /// Writes string to file. Avoids excessive fragmentation.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        public static void WriteToFile(string fileName, string content, System.Text.Encoding encoding = null)
        {
            if (encoding is null)
                encoding = System.Text.Encoding.Default;

            CheckFolder(fileName);

            using (var stream = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                using (var writer = new StreamWriter(stream, encoding))
                {
                    // first declare the amount of data we have 
                    stream.SetLength(content.Length);
                    
                    // then write
                    writer.Write(content);
                }
            }
        }

        /// <summary>
        /// Writes byte array to file. Avoids excessive fragmentation. 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        public static void WriteToFile(string fileName, byte[] data)
        {
            CheckFolder(fileName);

            using (var stream = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                // first declare the amount of data we have 
                stream.SetLength(data.Length);

                // then write
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

        // replaces back and forward slashes with system specific path separator
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

            if (!File.Exists(fileName))
            {
                Helpers.PanicError("Helpers", "Can't backup a file that doesnt exist!");
                return;
            }

            if (!File.Exists(backupName))
                File.Copy(fileName, backupName);
        }

        public static void BackupAndDeleteFile(string filename)
        {
            BackupFile(filename);
            File.Delete(filename);
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

        // extracts a file from the embedded Data.zip.
        // we only read compressed files from zip once, so no need for extra caching here
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

        public static Stream GetResourceAsStream(string resource)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            var stream = thisAssembly.GetManifestResourceStream($"CTRFramework.Data.{resource}");

            return stream;
        }

        /// <summary>
        /// Retrieves an array of lines from the embedded resource.
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

        public static XmlDocument LoadXml(string resource)
        {
            var doc = new XmlDocument();
            doc.LoadXml(Helpers.GetTextFromResource(resource));

            return doc;
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



        public static bool ContainsKana(string text)
        {
            foreach (char c in text.ToCharArray())
                if (c >= 0x3040 && c <= 0x30FF)
                    return true;

            return false;
        }

        public static byte FromBitArray(byte[] data)
        {
            byte result = 0;

            for (int i = 0; i < 8; i++)
                result |= (byte)(data[i] << i);

            return result;
        }

        public static Vector3 CloneVector(Vector3 vec) => new Vector3(vec.X, vec.Y, vec.Z);

        public static void Maximize(ref Vector3 src, Vector3 dst)
        {
            src.X = Math.Max(src.X, dst.X);
            src.Y = Math.Max(src.Y, dst.Y);
            src.Z = Math.Max(src.Z, dst.Z);
        }

        public static void Minimize(ref Vector3 src, Vector3 dst)
        {
            src.X = Math.Min(src.X, dst.X);
            src.Y = Math.Min(src.Y, dst.Y);
            src.Z = Math.Min(src.Z, dst.Z);
        }
    }
}