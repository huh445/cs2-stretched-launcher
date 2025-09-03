using System;
using System.Linq;
using System.Runtime.InteropServices;
using CS2StretchedLauncher.Services;
using CS2StretchedLauncher.Utilities;
using CS2StretchedLauncher.Tools;

namespace CS2StretchedLauncher
{
    internal static class Program
    {
        private static ResolutionManager? _res;
        private static GameLauncher? _launcher;
        private static class ConsoleHost
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern bool AllocConsole();
            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern bool FreeConsole();

            public static void Run(Action work)
            {
                AllocConsole();
                try { work(); }
                finally { FreeConsole(); }
            }
        }

        private static int Main(string[] args)
        {
            if (IsSettingsMode(args))
            {
                ConsoleHost.Run(() => ConsoleConfigurator.Run());
                return 0;
            }

            AppDomain.CurrentDomain.ProcessExit += (_, __) => SafeRestore();
            Console.CancelKeyPress += (_, __) => SafeRestore();

            try
            {
                var cfg = ConfigLoader.Load();

                _res = new ResolutionManager(
                    cfg.Low.Width,  cfg.Low.Height,  cfg.Low.Depth,
                    cfg.High.Width, cfg.High.Height, cfg.High.Depth);

                _launcher = new GameLauncher();

                if (!ApplyLowResolution())
                {
                    Logger.Log("Failed to set low resolution. Aborting launch.");
                    return 1;
                }

                _launcher.LaunchSteamUri("steam://rungameid/730");

                if (_launcher.WaitForProcessToAppear("cs2", TimeSpan.FromSeconds(30)))
                {
                    Logger.Log("CS2 launched. Monitoring until exit…");
                    _launcher.WaitWhileProcessExists("cs2", TimeSpan.FromSeconds(1));
                }
                else
                {
                    Logger.Log("CS2 process did not appear within 30s.");
                }

                _res.RestoreOriginalRes();
                return 0;
            }

            finally
            {
                SafeRestore();
            }
        }

        private static bool IsSettingsMode(string[] args) =>
            args.Any(a => string.Equals(a, "--settings", StringComparison.OrdinalIgnoreCase) ||
                          string.Equals(a, "config", StringComparison.OrdinalIgnoreCase));

        private static bool ApplyLowResolution()
        {
            return _res!.GetType().GetMethod("ApplyLowRes")?.Invoke(_res, null) as bool? ?? true;
            
        }

        private static void SafeRestore()
        {
            try { _res?.RestoreOriginalRes(); } catch { }
        }
    }
}
