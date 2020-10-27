using System;
using System.IO;
using System.Globalization;

namespace CTRFramework.Shared
{
    public class Helpers
    {
        public static bool panicEnabled = true;
        public static bool panicIntense = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">the object that wants to panic</param>
        /// <param name="msg">the message it wants to send</param>
        public static void Panic(object x, string msg)
        {
            if (panicEnabled)
            {
                Console.WriteLine(String.Format("{0}:\t{1}", x.GetType().Name, msg));
                if (panicIntense) Console.ReadKey();
            }
        }

        public static float Normalize(int min, int max, int val)
        {
            return ((float)val - (float)min) / ((float)max - (float)min);
        }

        public static Random Random = new Random();

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
        public static void WriteToFile(string fname, string content)
        {
            if (!Directory.Exists(Path.GetDirectoryName(fname)))
                Directory.CreateDirectory(Path.GetDirectoryName(fname));

            using (FileStream fs = new FileStream(fname, FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    fs.SetLength(content.Length);
                    sw.Write(content);
                }
            }
        }

        public static void WriteToFile(string fname, byte[] data)
        {
            if (!Directory.Exists(Path.GetDirectoryName(fname)))
                Directory.CreateDirectory(Path.GetDirectoryName(fname));

            using (FileStream fs = new FileStream(fname, FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    fs.SetLength(data.Length);
                    sw.Write(data);
                }
            }
        }

        public static DateTime ParseDate(string s)
        {
            DateTime result = DateTime.ParseExact(s.Replace("  ", " "), "ddd MMM d HH:mm:ss yyyy", CultureInfo.InvariantCulture);
            Console.WriteLine(result.ToString());
            return result;
        }
    }
}