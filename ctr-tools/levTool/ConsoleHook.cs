using System.IO;
using System.Text;
using System.Windows.Forms;

namespace levTool
{
    class ConsoleHook : TextWriter
    {
        TextBox _output = null;

        public ConsoleHook(TextBox output)
        {
            _output = output;
        }

        public override void Write(char value)
        {
            base.Write(value);
            _output.AppendText(value.ToString()); // When character data is written, append it to the text box.
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}