using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace CTRtools.Helpers
{

    //move this to Meta

    public struct MetaInst
    {
        public int Midi;
        public int Pitch;
        public int Key;
        public string Title;
    }

    class CTRJson
    {
        static string jsonpath = "ctrdata.json";

        //public static JArray levels;
        public static JObject midi;

        public static bool Load()
        {
            try
            {
                JObject json = JObject.Parse(File.ReadAllText(jsonpath));

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
                                return JsonConvert.DeserializeObject<MetaInst>(midi[track][inst][x].ToString());
                }
            }
            catch
            {
            }

            return new MetaInst();
        }
    }
}