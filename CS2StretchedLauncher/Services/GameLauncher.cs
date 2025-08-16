using CS2StretchedLauncher.Utilities;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace CS2StretchedLauncher.Services
{
    internal sealed class GameLauncher
    {
        public void LaunchSteamUri(string uri)
        {
            Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });
        }

        public bool WaitForProcessToAppear(string name, TimeSpan timeout)
        {
            var sw = Stopwatch.StartNew();
            while (sw.Elapsed < timeout)
            {
                if (Process.GetProcessesByName(name).Length > 0) return true;
                Thread.Sleep(500);
            }
            Logger.Log($"Process '{name}' did not appear within {timeout.TotalSeconds}s.");
            return false;
        }

        public void WaitWhileProcessExists(string name, TimeSpan pollInterval)
        {
            while (Process.GetProcessesByName(name).Length > 0)
                Thread.Sleep(pollInterval);
        }
    }
}
