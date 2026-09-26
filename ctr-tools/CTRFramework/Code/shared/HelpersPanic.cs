using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CTRFramework.Shared
{
    [Flags]
    public enum PanicLevel
    {
        Silent = 1 << 0,    //omits logger entirely
        Console = 1 << 1,   //writes message to the console
        Pause = 1 << 2,     //if console output enabled, additionaly stops and waits for user input
        File = 1 << 3,      //writes to file
        Exception = 1 << 4  //throws an exception for Error message
    }

    [Flags]
    public enum PanicType
    {
        None = 0,           //no messages
        All = -1,           //all messages
        Error = 1 << 0,     //anything considered abnormal or preventing code execution entirely
        Warning = 1 << 1,   //used to warn about unexpected code execution, i.e missing enum in switch case
        Info = 1 << 2,      //general info, messages
        Debug = 1 << 3,     //debug output
        Assume = 1 << 4,    //used for format assumptions or sanity checks
        Measure = 1 << 5    //used for time measurements, so you can only leave this on and check the results
    }

    /// <summary>
    /// Basically, a logger.
    /// Usage example: Helpers.Panic(this, PanicType.Assume, $"somevar = {somevar}: your message");
    /// </summary>
    public partial class Helpers
    {
        public static string logpath = Helpers.PathCombine(Meta.BasePath, "ctrframework.log");

        /// <summary>
        /// Defines the way all messages are handled. Can combine flags.
        /// </summary>
        public static PanicLevel panicLevel = PanicLevel.Console; //PanicLevel.File;

        /// <summary>
        /// Filters the messages to output. Can combine flags.
        /// </summary>
        public static PanicType panicType = PanicType.All; //^ PanicType.Debug;

        /// <summary>
        /// This overload wraps any object and retrieves it's to report who caused the panic..
        /// </summary>
        /// <param name="sender">the object that wants to panic</param>
        /// <param name="panicType">type of panic</param>
        /// <param name="message">the message it wants to send</param>
        public static void Panic(object sender, PanicType panicType, string message)
        {
            Panic(sender.GetType().Name, panicType, message);
        }

        /// <summary>
        /// Used to log Error events. Whenever something goes deliberately wrong and we know it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        public static void PanicError(object sender, string message)
        {
            Panic(sender.GetType().Name, PanicType.Error, message);
        }

        /// <summary>
        /// Used to log Assume events.
        /// Usually something we are not sure about or just want to visualize the unknown data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        public static void PanicAssume(object sender, string message)
        {
            Panic(sender.GetType().Name, PanicType.Assume, message);
        }

        /// <summary>
        /// Used to log debug events. Any info goes.
        /// Only included in a debug build.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        [Conditional("DEBUG")]
        public static void PanicDebug(object sender, string message)
        {
            // this dumps a ton of debug data, only use in debug builds
            Panic(sender.GetType().Name, PanicType.Debug, message);
        }

        /// <summary>
        /// A generic conditional panic.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="sender"></param>
        /// <param name="panicType"></param>
        /// <param name="message"></param>
        public static void PanicIf(bool condition, object sender, PanicType panicType, string message)
        {
            if (condition)
                Panic(sender, panicType, message);
        }

        public static StringBuilder Log = new StringBuilder();

        /// <summary>
        /// Root panic method. Call this to print anything.
        /// </summary>
        /// <param name="sender">the object that wants to panic</param>
        /// <param name="panicType">type of panic</param>
        /// <param name="message">the message it wants to send</param>
        public static void Panic(string sender, PanicType pType, string message)
        {
            // if silent mode, do nothing at all
            if (panicLevel.HasFlag(PanicLevel.Silent)) return;

            // set console text color, depending on the panic type
            switch (pType)
            {
                case PanicType.Debug: Console.ForegroundColor = ConsoleColor.DarkGray; break;
                case PanicType.Error: Console.ForegroundColor = ConsoleColor.Red; break;
                case PanicType.Warning: Console.ForegroundColor = ConsoleColor.DarkRed; break;
                case PanicType.Measure: Console.ForegroundColor = ConsoleColor.Blue; break;
                case PanicType.Info: Console.ForegroundColor = ConsoleColor.Gray; break;
                case PanicType.Assume: Console.ForegroundColor = ConsoleColor.Magenta; break;
                default: Console.ForegroundColor = ConsoleColor.White; break;
            }

            if (pType != PanicType.Info)
                message = $"{pType}\t{sender}:\t{message}";

            // maybe write it to file
            if (panicLevel.HasFlag(PanicLevel.File))
                Log.Append($"{DateTime.Now}\t{message}\r\n");

            // if we can print to console
            if (panicLevel.HasFlag(PanicLevel.Console))
            {
                // if this panic level is allowed to print
                if (panicType.HasFlag(pType))
                {
                    Console.WriteLine(message);

                    if (panicLevel.HasFlag(PanicLevel.Pause))
                        Console.ReadKey();
                }
            }

            if (panicLevel.HasFlag(PanicLevel.Exception))
                if (panicType.HasFlag(PanicType.Error))
                    throw new Exception(message);
        }

        /// <summary>
        /// Dumps log contents to disk and clears current log window.
        /// </summary>
        public static void DumpLog()
        {
            File.AppendAllText(logpath, Log.ToString());
            Log.Clear();
        }
    }
}