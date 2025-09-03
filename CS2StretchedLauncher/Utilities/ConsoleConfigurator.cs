using System;
using CS2StretchedLauncher.Utilities;

namespace CS2StretchedLauncher.Tools
{
    public static class ConsoleConfigurator
    {
        public static int Run()
        {
            var store = new JsonSettingsStore();
            var s = store.Load();

            Console.WriteLine("=== CS2 Stretched Launcher - Settings ===");
            Console.WriteLine($"Config path: {store.Path}");
            Console.WriteLine();

            s = s with { Low = PromptRes("Low", s.Low) };
            s = s with { High = PromptRes("High", s.High) };

            store.Save(s);
            Console.WriteLine();
            Console.WriteLine("Saved. Press any key to exit.");
            Console.ReadKey(true);
            return 0;
        }

        private static Resolution PromptRes(string label, Resolution current)
        {
            Console.WriteLine($"[{label} Resolution]");
            var w = PromptUInt($"Width ({current.Width})", current.Width);
            var h = PromptUInt($"Height ({current.Height})", current.Height);
            var d = PromptUInt($"Depth ({current.Depth})", current.Depth);
            Console.WriteLine();
            return new Resolution(w, h, d);
        }

        private static uint PromptUInt(string label, uint current)
        {
            while (true)
            {
                Console.Write($"{label}: ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) return current;
                if (uint.TryParse(input, out var v) && v > 0) return v;
                Console.WriteLine("  Enter a positive integer, or press Enter to keep current.");
            }
        }
    }
}