using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace CTRFramework.Shared
{
    public class Helpers
    {
        public static Random Random = new Random();

        public static string logpath = Path.Combine(Meta.BasePath, "ctrframework.log");

        public static PanicLevel panicLevel = PanicLevel.Console; //PanicLevel.File;
        public static PanicType panicType = PanicType.All;

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
        public static void Panic(string sender, PanicType panicType, string message)
        {
            if (panicLevel.HasFlag(PanicLevel.Silent))
                return;

            message = $"{panicType}\t{sender}:\t{message}";

            if (panicLevel.HasFlag(PanicLevel.File))
                File.AppendAllText(logpath, $"{DateTime.Now}\t{message}\r\n");

            if (panicLevel.HasFlag(PanicLevel.Console))
            {
                Console.WriteLine(message);

                if (panicLevel.HasFlag(PanicLevel.Pause))
                    Console.ReadKey();
            }

            if (panicLevel.HasFlag(PanicLevel.Exception) && panicType.HasFlag(PanicType.Error))
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

        public static DateTime ParseDate(string s)
        {
            DateTime result = DateTime.ParseExact(s.Replace("  ", " "), "ddd MMM d HH:mm:ss yyyy", CultureInfo.InvariantCulture);
            Console.WriteLine(result.ToString());
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
    }
}