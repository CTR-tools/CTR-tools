using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Collections.Generic;

namespace CTRFramework.Shared
{
    public class Meta
    {
        static string jsonpath = System.AppDomain.CurrentDomain.BaseDirectory + "\\versions.json";

        static JObject json;

        public static void Load()
        {
            if (File.Exists(jsonpath))
                json = JObject.Parse(File.ReadAllText(jsonpath));
        }

        public static string DetectBig(string file)
        {
            if (json == null) Load();

            Console.WriteLine("DetectBig({0})", Path.GetFileName(file));

            string md5 = Helpers.CalculateMD5(file);
            string res = "Unknown";

            Console.WriteLine("MD5 = {0}", md5);

            JToken j = json["files"][md5];
           // string tag = "ntsc";

            if (j != null)
            {
                res = j.Value<string>("list");

                Console.WriteLine(
                    String.Format("Detected BIG from {0}",
                    j.Value<string>("comment")
                    ));
            }
            else
            {
                Console.WriteLine("Unknown file.");
            }

            /*
            List<string> l = GetFileListByTag(tag);

            foreach (string s in l)
                Console.WriteLine(s);
                
            Console.ReadKey();
            */
            if (res == "Unknown")
                File.WriteAllText("unknown_md5.txt", md5);

            Console.WriteLine("list tag = {0}", res);
            return res;
        }

        public static List<string> GetFileListByTag(string tag)
        {
            List<string> result = new List<string>();

            switch (tag)
            {
                case "usa":
                    result.AddRange(GetLevelsList());
                    result.AddRange(GetBattleList());
                    break;
            }

            if (result.Count == 0)
                Console.WriteLine("Empty file list for bigfile.big");

            return result;
        }

        public static List<string> GetFileListByNumber(int x)
        {
            switch (x)
            {
                case 607: return GetFileListByTag("ntsc");
                default: return new List<string>();
            }
        }


        static string[] types = new string[] { "1P", "2P", "4P", "relic" };
        static string[] ext = new string[] { "lev", "vram" };


        static List<string> GetLevelsList()
        {
            List<string> result = new List<string>();

            string trackspath = "levels\\tracks";
            string[] levels = new string[] { 
                "canyon", "mines", "bluff", "cove",
                "temple", "pyramid", "tubes", "skyway",
                "sewer", "cave", "castle", "labs",
                "pass", "station", "park", "arena",
                "coliseum", "turbo"
            };


            foreach (string lev in levels)
                foreach (string typ in types)
                    foreach (string ex in ext)
                        result.Add($"{trackspath}\\{lev}\\{typ}\\data.{ex}");

            return result;
        }
        static List<string> GetBattleList()
        {
            List<string> result = new List<string>();

            string trackspath = "levels\\tracks";
            string[] levels = new string[] {
                "battle1", "battle2", "battle3", "battle4",
                "battle5", "battle6", "battle7"
            };

            foreach (string lev in levels)
                foreach (string typ in types)
                    foreach (string ex in ext)
                        result.Add($"{trackspath}\\{lev}\\{typ}\\data.{ex}");

            return result;
        }
    }
}
