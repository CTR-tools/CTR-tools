using System;
using System.Globalization;
using System.IO;

namespace CTRFramework.Shared
{
    public enum PanicLevel
    {
        Silent,
        Warn,
        Pause,
        Exception
    }

    public class Helpers
    {
        public static PanicLevel panic = PanicLevel.Warn;
        public static Random Random = new Random();

        /// <summary>
        /// Call this if something unexpected happened.
        /// </summary>
        /// <param name="x">the object that wants to panic</param>
        /// <param name="msg">the message it wants to send</param>
        public static void Panic(object x, string msg)
        {
            if (panic != PanicLevel.Silent)
            {
                if (panic == PanicLevel.Warn || panic == PanicLevel.Pause)
                {
                    Console.WriteLine($"{x.GetType().Name}:\t{msg}");

                    if (panic == PanicLevel.Pause)
                        Console.ReadKey();
                }

                if (panic == PanicLevel.Exception)
                    throw new Exception($"{x.GetType().Name}:\t{msg}");
            }
        }

        public static float Normalize(int min, int max, int val)
        {
            return ((float)val - (float)min) / ((float)max - (float)min);
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
        public static void WriteToFile(string fileName, string content)
        {
            CheckFolder(fileName);

            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(fs))
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

        public static void CheckFolder(string path)
        {
            if (Path.HasExtension(path))
                path = Path.GetDirectoryName(path);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}