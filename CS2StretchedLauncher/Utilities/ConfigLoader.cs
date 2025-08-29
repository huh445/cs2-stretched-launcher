using Microsoft.Extensions.Configuration;
using CS2StretchedLauncher.Services;

namespace CS2StretchedLauncher
{
    internal static class ConfigLoader
    {
        public static DisplaySettings Load(string filename = "settings.json")
        {
            // Build config from JSON
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile(filename, optional: true, reloadOnChange: true)
                .Build();

            var low = new DisplaySettings.Resolution(
                config.GetValue<uint>("Display:Low:Width", 1280),
                config.GetValue<uint>("Display:Low:Height", 720),
                config.GetValue<uint>("Display:Low:Depth", 32)
            );

            var high = new DisplaySettings.Resolution(
                config.GetValue<uint>("Display:High:Width", 1920),
                config.GetValue<uint>("Display:High:Height", 1080),
                config.GetValue<uint>("Display:High:Depth", 32)
            );

            return new DisplaySettings(low, high);
        }
    }
}