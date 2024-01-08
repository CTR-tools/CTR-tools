using System;
using System.Windows.Forms;
using System.IO;

namespace ctrviewer
{
    public class Program
    {
        public static bool Restart = false;

        [STAThread]
        public static void Main()
        {
            try
            {
                do
                {
                    Restart = false;

                    using (var game = new Game1())
                        game.Run();
                }
                while (Restart);
            }
            catch (Exception ex)
            {
                HandleGenericException(ex);
            }
        }

        public static void HandleGenericException(Exception ex)
        {
            var header = $"FATAL ERROR\r\n" +
                $"The following message is dumped to fatal_errors.txt.\r\n\r\n";

            var error = $"=====\r\n{ex.Message}\r\n\r\n{ex}\r\n=====\r\n\r\n";

            File.AppendAllText("fatal_errors.txt", error);

            MessageBox.Show(header + error, "Fatal error");
        }
    }
}