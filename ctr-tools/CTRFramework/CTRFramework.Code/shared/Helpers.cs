using System;
using System.Globalization;
using System.IO;

namespace CTRFramework.Shared
{
    [Flags]
    public enum PanicLevel
    {
        Silent = 1 << 0,        //silent level overrides other settings
        Warn = 1 << 1,          //writes message to the console
        Pause = 1 << 2,         //waits for user input
        File = 1 << 3,          //writes to file
        Exception = 1 << 4      //throws an exception
    }

    public class Helpers
    {
        public static string logpath = Path.Combine(Meta.BasePath, "ctrframework.log");

        public static PanicLevel panic = PanicLevel.Warn;
        public static Random Random = new Random();

        /// <summary>
        /// Call this if something unexpected happened.
        /// </summary>
        /// <param name="x">the object that wants to panic</param>
        /// <param name="msg">the message it wants to send</param>
        public static void Panic(object x, string msg)
        {
            if (panic.HasFlag(PanicLevel.Silent))
                return;

            string message = $"{x.GetType().Name}:\t{msg}";

            if (panic.HasFlag(PanicLevel.File))
                File.AppendAllText(logpath, DateTime.Now.ToString() + "\t" + message + "\r\n");

            if (panic.HasFlag(PanicLevel.Warn))
            {
                Console.WriteLine(message);

                if (panic.HasFlag(PanicLevel.Pause))
                    Console.ReadKey();
            }

            if (panic == PanicLevel.Exception)
                throw new Exception(message);
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

        public static byte TestPointer(uint ptr)
        {
            return (byte)(ptr & 3);
        }
    }
}