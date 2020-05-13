using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CTRTools
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
            _output.Invoke((MethodInvoker)delegate { _output.AppendText(value.ToString()); });
            Application.DoEvents();
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}