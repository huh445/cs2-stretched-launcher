using System;
using System.Diagnostics;

namespace CS2StretchedLauncher.Utilities
{
    internal static class Logger
    {
        [Conditional("DEBUG")]
        public static void Log(string message)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
        }
    }
}
