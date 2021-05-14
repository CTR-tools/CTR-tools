using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;


namespace ctrviewer.Engine
{
    [Serializable]
    public class EngineSettings
    {
        public static string SettingsFile = "settings.xml";

        public Point Resolution = Point.Zero;

        public byte AntiAliasLevel { get; set; } = 4;
        public bool TextureFiltering { get; set; } = true;
        public bool VisData { get; set; } = false;
        public bool VisDataLeaves { get; set; } = false;
        public bool GenerateMips { get; set; } = true;
        public bool ShowSky { get; set; } = true;
        public bool ShowBotsPath { get; set; } = false;
        public bool ShowModels { get; set; } = false;
        public bool StereoPair { get; set; } = false;
        public int StereoPairSeparation { get; set; } = 20;
        public bool ShowCamPos { get; set; } = false;
        public bool UseLowLod { get; set; } = false;
        public bool ShowConsole { get; set; } = false;

        private bool _windowed = true;
        public bool Windowed
        {
            get
            {
                return _windowed;
            }
            set
            {
                _windowed = value;
                if (onWindowedChanged != null)
                    onWindowedChanged();
            }
        }

        private bool _vertexLighting = true;
        public bool VertexLighting
        {
            get
            {
                return _vertexLighting;
            }
            set
            {
                _vertexLighting = value;
                if (onVertexLightingChanged != null)
                    onVertexLightingChanged();
            }
        }

        private bool _antiAlias = true;
        public bool AntiAlias
        {
            get
            {
                return _antiAlias;
            }
            set
            {
                _antiAlias = value;
                if (onAntiAliasChanged != null)
                    onAntiAliasChanged();
            }
        }

        private bool _verticalSync = true;
        public bool VerticalSync
        {
            get
            {
                return _verticalSync;
            }
            set
            {
                _verticalSync = value;
                if (onVerticalSyncChanged != null)
                    onVerticalSyncChanged();
            }
        }

        private int _fieldOfView = 80;
        public int FieldOfView
        {
            get
            {
                if (_fieldOfView < 20) _fieldOfView = 20;
                if (_fieldOfView > 150) _fieldOfView = 150;
                return _fieldOfView;
            }
            set
            {
                _fieldOfView = value;
                if (onFieldOfViewChanged != null)
                    onFieldOfViewChanged();
            }
        }

        private int _windowScale = 75;
        public int WindowScale
        {
            get
            {
                if (_windowScale < 10) _fieldOfView = 10;
                if (_windowScale > 90) _fieldOfView = 90;
                return _fieldOfView;
            }
            set
            {
                _fieldOfView = value;
                if (onWindowedChanged != null)
                    onWindowedChanged();
            }
        }

        public delegate void DelegateNoArgs();

        [XmlIgnore]
        public DelegateNoArgs onWindowedChanged = null;
        [XmlIgnore]
        public DelegateNoArgs onVertexLightingChanged = null;
        [XmlIgnore]
        public DelegateNoArgs onAntiAliasChanged = null;
        [XmlIgnore]
        public DelegateNoArgs onVerticalSyncChanged = null;
        [XmlIgnore]
        public DelegateNoArgs onFieldOfViewChanged = null;

        public EngineSettings()
        {
        }

        public void Save(string filename = "")
        {
            if (filename == "")
                filename = EngineSettings.SettingsFile;

            using (StreamWriter sw = new StreamWriter(File.Create(filename)))
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                XmlSerializer x = new XmlSerializer(this.GetType());
                x.Serialize(sw, this, ns);
                sw.Flush();
                sw.Close();
            }
        }

        public static EngineSettings Load(string filename = "")
        {
            if (filename == "")
                filename = EngineSettings.SettingsFile;

            EngineSettings settings = new EngineSettings();

            if (!File.Exists(filename))
                return settings;

            using (StreamReader reader = new StreamReader(File.OpenRead(filename)))
            {
                try
                {
                    XmlSerializer x = new XmlSerializer(settings.GetType());
                    settings = (EngineSettings)x.Deserialize(reader);
                    reader.Close();
                }
                catch
                {
                    GameConsole.Write("Load settings failed.");
                }
            }

            return settings;
        }
    }
}
