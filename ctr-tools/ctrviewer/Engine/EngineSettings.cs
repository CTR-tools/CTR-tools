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
        //        public static EngineSettings Instance => (singleton == null ? Load() : singleton);

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
        public string BigFileLocation = $".\\{Meta.BigFileName}";

        private bool _internalPSXResolution = false;
        public bool InternalPSXResolution
        {
            get => _internalPSXResolution;
            set
            {
                _internalPSXResolution = value;
                onInternalPsxResolutionChanged?.Invoke();
            }
        }

        private int _anisotropyLevel = 4;
        public int AnisotropyLevel
        {
            get => _anisotropyLevel;
            set
            {
                UpdateIntValue(ref _anisotropyLevel, value, 0, 4);
                onAnisotropyChanged?.Invoke();
            }
        }

        public bool TextureFiltering { get; set; } = true;

        public int AntiAliasLevel { get; set; } = 4;

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
        public bool StereoCrossEyed { get; set; } = true;
        public string PlayerModel { get; set; } = "crash";
        public float StereoPairSeparation { get; set; } = 10;
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
            get => _vertexLighting;
            set
            {
                _vertexLighting = value;
                onVertexLightingChanged?.Invoke();
            }
        }

        private bool _antiAlias = true;
        public bool AntiAlias
        {
            get => _antiAlias;
            set
            {
                _antiAlias = value;
                onAntiAliasChanged?.Invoke();
            }
        }

        private bool _verticalSync = true;
        public bool VerticalSync
        {
            get => _verticalSync;
            set
            {
                _verticalSync = value;
                onVerticalSyncChanged?.Invoke();
            }
        }

        private int _fieldOfView = 80;
        public int FieldOfView
        {
            get => _fieldOfView;
            set
            {
                UpdateIntValue(ref _fieldOfView, value, 20, 150);
                onFieldOfViewChanged?.Invoke();
            }
        }

        private int _windowScale = 75;
        public int WindowScale
        {
            get => _windowScale;
            set
            {
                UpdateIntValue(ref _windowScale, value, 10, 90);
                onWindowedChanged?.Invoke();
            }
        }

        public delegate void DelegateNoArgs();

        [XmlIgnore]
        public DelegateNoArgs onAnisotropyChanged = null;
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

        public void UpdateIntValue(ref int value, int newvalue, int min, int max)
        {
            value = newvalue;
            if (newvalue < min) value = min;
            if (newvalue > max) value = max;
        }

        public static void Save(string filename = "")
        {
            if (filename == "")
            {
                Helpers.CheckFolder(Meta.UserPath);
                filename = Meta.SettingsFile;
            }

            var settings = new XmlWriterSettings() { Indent = true };

            using (var sw = XmlWriter.Create(filename, settings))
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                var x = new XmlSerializer(Instance.GetType());
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

            var settings = new EngineSettings();

            if (!File.Exists(filename))
                return settings;

            using (var reader = new StreamReader(File.OpenRead(filename)))
            {
                try
                {
                    var x = new XmlSerializer(settings.GetType());
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
