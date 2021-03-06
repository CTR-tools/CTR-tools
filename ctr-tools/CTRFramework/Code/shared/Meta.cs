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
        public static string JsonPath = "versions.json";
        public static string XmlPath = "versions.xml";
        public static string HowlPath = "howlnames.txt";
        public static string CseqPath = "cseq.json";
        public static string SmplPath = "samplenames.txt";
        public static string ModelsPath = "models";


        static JObject json;

        public static void Load()
        {
            json = JObject.Parse(Helpers.GetTextFromResource(JsonPath));
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

        public static Dictionary<int, string> GetBigList(string resource)
        {
            return Meta.LoadNumberedList(resource); //Path.Combine(Meta.DataPath, fn));
        }

        public static Dictionary<int, string> LoadNumberedList(string resource)
        {
            string[] lines = Helpers.GetLinesFromResource(resource);

            Dictionary<int, string> names = new Dictionary<int, string>();

            foreach (string line in lines)
            {
                if (line.Trim() != "" && !line.Contains("#"))
                {
                    string[] bb = line.Replace(" ", "").Split('=');

                    int x = -1;
                    Int32.TryParse(bb[0], out x);

                    if (x == -1)
                    {
                        Console.WriteLine("List parsing error at: {0}", line);
                        continue;
                    }

                    names.Add(x, bb[1]);
                }
            }

            return names;
        }

        public static string GetVersion()
        {
            return "CTRFramework " + p.Version + " (" + p.BuildDate.Split(',')[0] + ")";
        }

        public static string GetSignature()
        {
            return p.signature;
        }
    }
}
