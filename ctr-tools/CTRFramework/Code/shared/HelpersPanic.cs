using System;
using System.IO;
using System.Diagnostics;

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
        public static PanicType panicType = PanicType.All ^ PanicType.Debug; //PanicType.All;

        /// <summary>
        /// Call this if something unexpected happened. Takes "this" and converts to class name string.
        /// </summary>
        /// <param name="sender">the object that wants to panic</param>
        /// <param name="panicType">type of panic</param>
        /// <param name="message">the message it wants to send</param>
        //[Conditional("DEBUG")]
        public static void Panic(object sender, PanicType panicType, string message)
        {
            Panic(sender.GetType().Name, panicType, message);
        }

        public static void PanicError(object sender, string message)
        {
            Panic(sender.GetType().Name, PanicType.Error, message);
        }

        public static void PanicAssume(object sender, string message)
        {
            Panic(sender.GetType().Name, PanicType.Assume, message);
        }

        public static void PanicDebug(object sender, string message)
        {
            Panic(sender.GetType().Name, PanicType.Debug, message);
        }


        //[Conditional("DEBUG")]
        public static void PanicIf(bool condition, object sender, PanicType panicType, string message)
        {
            if (condition)
                Panic(sender, panicType, message);
        }

        /// <summary>
        /// Call this if something unexpected happened.
        /// </summary>
        /// <param name="sender">the object that wants to panic</param>
        /// <param name="panicType">type of panic</param>
        /// <param name="message">the message it wants to send</param>
        //[Conditional("DEBUG")]
        public static void Panic(string sender, PanicType pType, string message)
        {
            if (panicLevel.HasFlag(PanicLevel.Silent))
                return;

            if (pType != PanicType.Info)
                message = $"{pType}\t{sender}:\t{message}";

            if (panicLevel.HasFlag(PanicLevel.File))
                File.AppendAllText(logpath, $"{DateTime.Now}\t{message}\r\n");

            if (panicLevel.HasFlag(PanicLevel.Console))
            {
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
    }
}