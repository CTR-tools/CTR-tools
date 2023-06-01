namespace ctrviewer.Engine.Gui
{
    public class StringMenuItem : MenuItem
    {
        private string _value = "";

        public new string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public StringMenuItem(string InitValue)
        {
            _value = InitValue;
        }
    }
}