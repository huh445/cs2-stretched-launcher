using CS2StretchedLauncher.Services;
using CS2StretchedLauncher.Utilities;
using System;
using System.ComponentModel;
using Microsoft.Extensions.Configuration;

namespace CS2StretchedLauncher
{
    internal static class Program
    {

        private static ResolutionManager? _res;
        private static GameLauncher? _launcher;

        private static int Main()
        {
            var _low = new DisplaySettings.Resolution();
            var _high = new DisplaySettings.Resolution();

            _res = new ResolutionManager(
                _low.Width, _low.Height, _low.Depth,
                _high.Width, _high.Height, _high.Depth
            );

            _launcher = new GameLauncher();

            AppDomain.CurrentDomain.ProcessExit += (_, __) => SafeRestore();
            Console.CancelKeyPress += (_, e) => { e.Cancel = true; SafeRestore(); Environment.Exit(0); };

            if (!_res.ChangeRes())
            {
                Logger.Log($"Failed to set {_low.Width}x{_low.Height}. Aborting.");
                return 2;
            }

            try
            {
                try
                {
                    _launcher.LaunchSteamUri("steam://rungameid/730");
                }
                catch (Win32Exception ex)
                {
                    Logger.Log("Failed to open Steam URI: " + ex.Message);
                    return 3;
                }

                if (_launcher.WaitForProcessToAppear("cs2", TimeSpan.FromSeconds(60)))
                {
                    _launcher.WaitWhileProcessExists("cs2", TimeSpan.FromMilliseconds(750));
                }

                return 0;
            }

            finally
            {
                _res.RestoreOriginalRes();
            }
        }

        private static void SafeRestore()
        {
            _res?.RestoreOriginalRes();
        }
    }
}
