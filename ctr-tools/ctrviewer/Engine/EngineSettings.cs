using CTRFramework.Shared;
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
        private static EngineSettings singleton = null;

        public static EngineSettings Instance
        {
            get
            {
                if (singleton == null)
                    singleton = EngineSettings.Load();

                return singleton;
            }
        }

        public Point Resolution = Point.Zero;

        private bool _internalPSXResolution = false;
        public bool InternalPSXResolution
        {
            get
            {
                return _internalPSXResolution;
            }
            set
            {
                _internalPSXResolution = value;
                onInternalPsxResolutionChanged?.Invoke();
            }
        }

        public string BigFileLocation = ".\\bigfile.big";

        public byte AntiAliasLevel { get; set; } = 4;
        public bool TextureFiltering { get; set; } = true;

        public bool UseTextureReplacements { get; set; } = false;
        public bool VisData { get; set; } = false;
        public bool VisDataLeaves { get; set; } = false;
        public bool GenerateMips { get; set; } = true;
        public bool ShowSky { get; set; } = true;
        public bool BackFaceCulling { get; set; } = true;
        public bool ShowWater { get; set; } = true;
        public bool ShowInvisible { get; set; } = false;
        public bool ShowBotPaths { get; set; } = false;
        public bool ShowModels { get; set; } = true;
        public bool KartMode { get; set; } = false;
        public bool DrawWireframe { get; set; } = false;

        private bool _enableFiltering = true;
        public bool EnableFiltering
        {
            get
            {
                return _enableFiltering;
            }
            set
            {
                _enableFiltering = value;
                onFilteringChanged?.Invoke();
            }
        }

        public bool StereoPair { get; set; } = false;
        public string PlayerModel { get; set; } = "crash";
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
                onWindowedChanged?.Invoke();
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
                onVertexLightingChanged?.Invoke();
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
                onAntiAliasChanged?.Invoke();
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
                onVerticalSyncChanged?.Invoke();
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
                onFieldOfViewChanged?.Invoke();
            }
        }

        private int _windowScale = 75;
        public int WindowScale
        {
            get
            {
                if (_windowScale < 10) _windowScale = 10;
                if (_windowScale > 90) _windowScale = 90;
                return _windowScale;
            }
            set
            {
                _windowScale = value;
                onWindowedChanged?.Invoke();
            }
        }

        public delegate void DelegateNoArgs();

        [XmlIgnore]
        public DelegateNoArgs onFilteringChanged = null;
        [XmlIgnore]
        public DelegateNoArgs onInternalPsxResolutionChanged = null;
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

        public static void Save(string filename = "")
        {
            if (filename == "")
            {
                Helpers.CheckFolder(Meta.UserPath);
                filename = Meta.SettingsFile;
            }

            XmlWriterSettings settings = new XmlWriterSettings() { Indent = true };

            using (XmlWriter sw = XmlWriter.Create(filename, settings))
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                XmlSerializer x = new XmlSerializer(Instance.GetType());
                x.Serialize(sw, Instance, ns);
                sw.Flush();
                sw.Close();
            }
        }

        public static EngineSettings Load(string filename = "")
        {
            if (filename == "")
            {
                Helpers.CheckFolder(Meta.UserPath);
                filename = Meta.SettingsFile;
            }

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
