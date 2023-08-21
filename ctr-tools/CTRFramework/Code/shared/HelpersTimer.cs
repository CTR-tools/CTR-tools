using System.Collections.Generic;
using System.Diagnostics;

namespace CTRFramework.Shared
{
    public class HelpersTimer
    {
        private static Dictionary<string, Stopwatch> timers = new Dictionary<string, Stopwatch>();

        public static Stopwatch Create(string label)
        {
            var sw = new Stopwatch();
            timers.Add(label, sw);
            return sw;
        }

        public static int Stop(string label)
        {
            timers[label]?.Stop();
            int i = (int)timers[label].ElapsedMilliseconds;
            timers.Remove(label);
            return i;
        }
    }
}
