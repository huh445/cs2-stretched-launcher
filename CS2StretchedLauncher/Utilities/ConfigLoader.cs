using Microsoft.Extensions.Configuration; // still referenced for future, but we now rely on store
using CS2StretchedLauncher.Services;

namespace CS2StretchedLauncher.Utilities
{
    internal static class ConfigLoader
    {

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