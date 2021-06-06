namespace ctrviewer.Engine
{
    public partial class MainEngine
    {
        public EngineSettings Settings
        {
            get
            {
                if (settings == null)
                    settings = EngineSettings.Load();

                return settings;
            }
        }
    }
}