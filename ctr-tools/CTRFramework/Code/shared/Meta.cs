using System;
using System.Collections.Generic;
using System.Xml;
using resources = CTRFramework.Properties.Resources;

namespace CTRFramework.Shared
{
    public class Meta
    {
        public static int SectorSize = 0x800;

        #region [Paths/filenames]
        public static string BasePath = AppDomain.CurrentDomain.BaseDirectory;
        public static string UserPath = Helpers.PathCombine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CTRViewer");
        public static string SettingsFile = Helpers.PathCombine(UserPath, "settings.xml");
        public const string XmlPath = "versions.xml";
        public const string HowlPath = "howlnames.txt";
        public const string CseqPath = "cseq.json";
        public const string CseqXmlPath = "cseq.xml";
        public const string SmplPath = "samplenames.txt";
        public const string BankPath = "banknames.txt";
        public const string ModelsPath = "models";
        public const string BigFileName = "bigfile.big";
        public const string BashPath = "bash_filelist.txt";

        public const string LinkDiscord = "https://discord.gg/56xm9Aj";
        public const string LinkGithub = "https://github.com/CTR-tools/CTR-tools";
        #endregion

        public static string GetVersion() => $"CTRFramework {resources.Version} ({resources.BuildDate.Split(',')[0]})";

        public static string GetSignature() => resources.signature;


        static XmlDocument midixml;

        public static bool LoadMidiJson()
        {
            try
            {
                midixml = new XmlDocument();
                midixml.LoadXml(Helpers.GetTextFromResource(CseqXmlPath));

                return true;
            }
            catch (Exception ex)
            {
                Helpers.Panic("Meta", PanicType.Error, $"Failed to load meta instruments: {ex.Message}");
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

        public static MetaInst GetMetaInst(string song, string inst, int index)
        {
            if (midixml == null)
                if (!Meta.LoadMidiJson())
                    return new MetaInst();

            try
            {
                var node = midixml.SelectNodes($"/midi/song[@title='{song}']");

                if (node == null)
                {
                    Helpers.Panic("Meta", PanicType.Warning, $"Missing song in cseq.xml: {song}");
                    return new MetaInst();
                }

                var insts = midixml.SelectNodes($"/midi/song[@title='{song}']/{inst}[{index+1}]"); //looks like xpath numbering starts from 1?

                if (insts.Count == 1)
                {
                    //this is better to implement via xml serialization later
                    var zzz = new MetaInst();

                    if (insts[0].Attributes["title"] != null)
                        zzz.Title = insts[0].Attributes["title"].Value;

                    if (insts[0].Attributes["pitch"] != null)
                        zzz.Pitch = Int32.Parse(insts[0].Attributes["pitch"].Value);

                    if (insts[0].Attributes["key"] != null)
                        zzz.Key = Int32.Parse(insts[0].Attributes["key"].Value);

                    if (insts[0].Attributes["midi"] != null)
                        zzz.Midi = Int32.Parse(insts[0].Attributes["midi"].Value);

                    return zzz;
                }
            }
            catch (Exception ex)
            {
                Helpers.Panic("Meta", PanicType.Error, $"Failed to load meta instrument: {song} {inst} {index}\r\n{ex.Message}");
            }
                
            return new MetaInst();
        }

        //returns list of available songs in cseq.xml
        public static List<string> GetPatchList()
        {
            var list = new List<string>();

            foreach (XmlElement el in midixml.SelectNodes($"/midi/song"))
                list.Add(el.Attributes["title"].Value);

            return list;
        }

        public static string GetMetaInstText(string name)
        {
            return "implement me!"; //midi[name].ToString();
        }

        public static int GetBankIndex(string track)
        {
            //did we mean to support custom midi drum banks here???

            return 0;

            /*
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
            */
        }
    }
    public struct MetaInst
    {
        public int Midi { get; set; }
        public int Pitch { get; set; }
        public int Key { get; set; }
        public string Title { get; set; }
    }
}
