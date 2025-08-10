using CS2StretchedLauncher.Services;
using CS2StretchedLauncher.Utilities;
using System;
using System.ComponentModel;

namespace CS2StretchedLauncher
{
    internal static class Program
    {
        // Hardcoded per your request
        private const uint LOW_W = 1280, LOW_H = 960, LOW_BPP = 32;
        private const uint HIGH_W = 1920, HIGH_H = 1080, HIGH_BPP = 32;

        private static ResolutionManager? _res;
        private static GameLauncher? _launcher;

        private static int Main()
        {
            _res = new ResolutionManager(LOW_W, LOW_H, LOW_BPP, HIGH_W, HIGH_H, HIGH_BPP);
            _launcher = new GameLauncher();

            AppDomain.CurrentDomain.ProcessExit += (_, __) => SafeRestore();
            Console.CancelKeyPress += (_, e) => { e.Cancel = true; SafeRestore(); Environment.Exit(0); };

            // 1) Set desktop to 1280×960 (QRes-style: primary device, commit to registry, no forced Hz)
            if (!_res.ApplyLow())
            {
                Logger.Log("Failed to set 1280x960. Aborting.");
                return 2;
            }

            try
            {
                // 2) Launch CS2 via Steam
                try
                {
                    _launcher.LaunchSteamUri("steam://rungameid/730");
                }
                catch (Win32Exception ex)
                {
                    Logger.Log("Failed to open Steam URI: " + ex.Message);
                    return 3;
                }

                // 3) Wait for cs2 to start (up to 60s), then wait while running
                if (_launcher.WaitForProcessToAppear("cs2", TimeSpan.FromSeconds(60)))
                {
                    _launcher.WaitWhileProcessExists("cs2", TimeSpan.FromMilliseconds(750));
                }

                return 0;
            }
            finally
            {
                // 4) Restore desktop to 1920×1080
                _res.RestoreHigh();
            }
        }

        private static void SafeRestore()
        {
            try { _res?.RestoreHigh(); } catch { /* swallow */ }
        }
    }
}
