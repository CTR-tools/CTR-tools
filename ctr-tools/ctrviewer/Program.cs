using System;

namespace ctrviewer
{
    public class Program
    {
        public static bool Restart = false;

        [STAThread]
        public static void Main()
        {
            do
            {
                Restart = false;

                using (var game = new Game1())
                    game.Run();
            }
            while (Restart);
        }
    }
}