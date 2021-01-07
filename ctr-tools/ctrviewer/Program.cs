using System;

namespace ctrviewer
{
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (var game = new Game1())
                game.Run();
        }
    }
}