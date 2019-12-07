using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace CTRFramework.Shared
{
    public class Meta
    {
        static string jsonpath = System.AppDomain.CurrentDomain.BaseDirectory + "\\versions.json";

        public static string DetectBig(string file)
        {
            Console.WriteLine("DetectBig({0})", file);

            string md5 = Helpers.CalculateMD5(file);
            string res = "Unknown";

            Console.WriteLine("MD5 = {0}", md5);

            if (File.Exists(jsonpath))
            {
                JObject json = JObject.Parse(File.ReadAllText(jsonpath));

                for (int i = 0; i < ((JArray)json["versions"]).Count; i++)
                {
                    if (md5 == (string)json["versions"][i]["big_md5"])
                    {
                        res = String.Format(
                                "{0} ({1})",
                                json["versions"][i]["name"].ToString(),
                                json["versions"][i]["timestamp"].ToString()
                                );
                    }
                }
            }

            if (res == "Unknown")
                File.WriteAllText("unknown_md5.txt", md5);

            Console.WriteLine("result = {0}", res);
            return res;
        }

    }
}
