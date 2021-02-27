using CTRFramework.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace CTRFramework.Sound.CSeq
{

    //move this to Meta

    public struct MetaInst
    {
        public int Midi;
        public int Pitch;
        public int Key;
        public string Title;
    }

    public class CTRJson
    {
        //public static JArray levels;
        public static JObject midi;

        public static bool Load()
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
                                return JsonConvert.DeserializeObject<MetaInst>(midi[track][inst][x].ToString());
                }
            }
            catch
            {
            }

            return new MetaInst();
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
}