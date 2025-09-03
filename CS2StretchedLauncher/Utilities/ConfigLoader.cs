using Microsoft.Extensions.Configuration; // still referenced for future, but we now rely on store
using CS2StretchedLauncher.Services;
using CS2StretchedLauncher.Utilities;

namespace CS2StretchedLauncher
{
    internal static class ConfigLoader
    {
        /// <summary>
        /// Loads settings from %APPDATA% and adapts to DisplaySettings (used by the rest of the app).
        /// </summary>
        public static DisplaySettings Load(string _ = null!)
        {
            var store = new JsonSettingsStore();
            var app = store.Load();

            var low = new DisplaySettings.Resolution(app.Low.Width, app.Low.Height, app.Low.Depth);
            var high = new DisplaySettings.Resolution(app.High.Width, app.High.Height, app.High.Depth);
            Logger.Log($"Low res: {low.Width}x{low.Height}");

            return new DisplaySettings(low, high);
        }
    }
}