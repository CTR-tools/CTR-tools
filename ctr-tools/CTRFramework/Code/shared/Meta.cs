using Newtonsoft.Json;
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
        public static string BankPath = "banknames.txt";
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

                    if (!names.ContainsKey(x))
                    {
                        names.Add(x, bb[1]);
                    }
                    else
                    {
                        Helpers.Panic("Meta", PanicType.Error, $"duplicate entry {x}");
                    }
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





        //public static JArray levels;
        public static JObject midi;

        public static bool LoadMidiJson()
        {
            try
            {
                JObject json = JObject.Parse(Helpers.GetTextFromResource(Meta.CseqPath));

                //levels = (JArray)json["levels"];
                midi = (JObject)json["midi"];

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return false;
            }
        }
        /*
        public static string GetLevelTitle(string lev)
        {
            foreach (JToken j in levels)
                if (j["name"].ToString() == lev)
                    return j["title"].ToString();

            return "!" + lev + "!";
        }
        */
        public static MetaInst GetMetaInst(string track, string inst, int x)
        {
            try
            {
                //really?
                if (midi != null)
                {
                    if (midi[track] != null)
                        if (midi[track][inst] != null)
                            if (midi[track][inst][x] != null)
                            {
                                return JsonConvert.DeserializeObject<MetaInst>(midi[track][inst][x].ToString());
                            }
                }
            }
            catch
            {
            }

            return new MetaInst();
        }

        public static List<string> GetPatchList()
        {
            var list = new List<string>();

            foreach (KeyValuePair<string, JToken> j in midi)
                list.Add(j.Key);

            return list;
        }

        public static int GetBankIndex(string track)
        {
            try
            {
                //really?
                if (midi != null)
                {
                    if (midi[track] != null)
                    {
                        if (midi[track]["bank"] != null)
                        {
                            return midi[track]["bank"].ToObject<int>();
                        }
                    }
                }
            }
            catch
            {
            }

            return 0;
        }

    }


    public struct MetaInst
    {
        public int Midi;
        public int Pitch;
        public int Key;
        public string Title;
    }

}
