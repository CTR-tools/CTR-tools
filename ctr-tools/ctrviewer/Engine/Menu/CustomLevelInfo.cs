using CTRFramework.Shared;
using System;
using System.IO;
using System.Xml.Serialization;

namespace ctrviewer.Engine.Menu
{
    [Serializable]
    public class CustomLevelInfo
    {
        public string LevelName { get; set; } = "";
        public string LevelType { get; set; } = "";
        public string LevelFile { get; set; } = "";
        public string VramFile { get; set; } = "";
        public string ThumbnailImage { get; set; } = "";
        public string Directory { get; set; } = "";
        public string FullLevelPath => (Directory != null && LevelFile != null) ? Helpers.PathCombine(Directory, LevelFile) : "";

        public override string ToString()
        {
            return $"{LevelName}, {LevelType}, {LevelFile}, {VramFile}, {ThumbnailImage}, {Directory}";
        }

        public static CustomLevelInfo FromFile(string filename = "")
        {
            if (filename == "") return null;
            if (!File.Exists(filename)) return null;

            var info = new CustomLevelInfo();

            using (var reader = new StreamReader(File.OpenRead(filename)))
            {
                try
                {
                    var x = new XmlSerializer(info.GetType());
                    info = (CustomLevelInfo)x.Deserialize(reader);
                    reader.Close();
                    x = null;
                }
                catch
                {
                    GameConsole.Write("Failed to parse custom level info.");
                }
            }

            info.Directory = Path.GetDirectoryName(filename);

            return info;
        }
    }
}