using System;
using System.IO;

namespace CTRFramework.Shared
{
    public class Helpers
    {
        public static string CalculateMD5(string filename)
        {
            using (var stream = File.OpenRead(filename))
            {
                using (var md5 = System.Security.Cryptography.MD5.Create())
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        public static string CalculateMD5(MemoryStream ms)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var hash = md5.ComputeHash(ms);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }


        //avoids excessive fragmentation
        public static void WriteToFile(string fname, string content)
        {
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
            using (FileStream fs = new FileStream(fname, FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    fs.SetLength(data.Length);
                    sw.Write(data);
                }
            }
        }
    }
}
