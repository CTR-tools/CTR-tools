using System.Text;

namespace cseq.Helpers
{
    class Log
    {

        static StringBuilder sb = new StringBuilder();

        public static void Write(string s)
        {
            sb.Append(s);
        }

        public static void WriteLine(string s)
        {
            sb.Append(s + "\r\n");
        }

        public static void Clear()
        {
            sb.Clear();
        }

        public static string Read()
        {
            return sb.ToString();
        }
    }
}
