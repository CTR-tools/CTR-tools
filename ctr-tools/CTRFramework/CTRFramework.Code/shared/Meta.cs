using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using p = CTRFramework.Properties.Resources;

namespace CTRFramework.Shared
{
    public class Meta
    {
        public static int SectorSize = 0x800;

        public static string BasePath = System.AppDomain.CurrentDomain.BaseDirectory;
        public static string DataPath = Path.Combine(BasePath, "CTRFramework.Data");
        public static string JsonPath = Path.Combine(DataPath, "versions.json");
        public static string HowlPath = Path.Combine(DataPath, "howlnames.txt");
        public static string CseqPath = Path.Combine(DataPath, "cseq.json");
        public static string SmplPath = Path.Combine(DataPath, "samplenames.txt");

        static JObject json;

        public static void Load()
        {
            if (File.Exists(JsonPath))
                json = JObject.Parse(File.ReadAllText(JsonPath));
        }

        public List<string> LoadList(string fn)
        {
            string[] s = File.ReadAllLines(fn);
            return new List<string>(s);
        }

        public static string Detect(string file, string list, int fc)
        {
            if (json == null) Load();

            Console.Write("Calculating MD5... ");

            string md5 = Helpers.CalculateMD5(file);
            string res = "";

            Console.WriteLine(md5);

            JToken j = json[list][md5];
            // string tag = "ntsc";

            if (j != null)
            {
                res = j.Value<string>("list");

                Console.WriteLine(
                    String.Format("Detected file from {0}",
                    j.Value<string>("comment")
                    ));
            }
            else
            {
                Console.WriteLine("Unknown file.");

                j = json["big_nums"][fc.ToString()];

                if (j != null)
                {
                    res = j.ToString();

                    Console.WriteLine(
                        String.Format("{0} files in BIG. Assume: {1}",
                        fc, res
                        ));
                }
            }

            if (res == "")
                File.WriteAllText("unknown_md5.txt", md5);

            Console.WriteLine("list tag = {0}", res);
            return res;
        }

        public static Dictionary<int, string> GetProperBigList(string fn, int fc)
        {
            string s = Meta.Detect(fn, "bigs", fc);
            return Meta.LoadNumberedList(Path.Combine(Meta.DataPath, s + ".txt"));
        }

        public static Dictionary<int, string> LoadNumberedList(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    Dictionary<int, string> names = new Dictionary<int, string>();

                    string[] buf = File.ReadAllLines(path);

                    foreach (string b in buf)
                    {
                        if (b.Trim() != "" && !b.Contains("#"))
                        {
                            string[] bb = b.Replace(" ", "").Split('=');

                            int x = -1;
                            Int32.TryParse(bb[0], out x);

                            if (x == -1)
                            {
                                Console.WriteLine("List parsing error at: {0}", b);
                                continue;
                            }

                            names.Add(x, bb[1]);
                        }
                    }

                    return names;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw new Exception("Failed to access or parse data file: " + path);
                }
            }
            else
            {
                Console.WriteLine("File doesn't exist: " + path);
                return new Dictionary<int, string>();
            }

        }

        public static string GetVersionInfo()
        {
            return "CTRFramework " + p.Version + " (" + p.BuildDate.Split(',')[0] + ")";
        }
    }
}
