using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using CS2StretchedLauncher.Utilities;

namespace CS2StretchedLauncher.Services
{

    internal sealed class DisplaySettings
    {
        public readonly record struct Resolution(uint Width, uint Height, uint Depth);
        public Resolution Low { get; }
        public Resolution High { get; }
        public DisplaySettings()
        {
            var config = ConfigService();
            (var lowWidth, var lowHeight) = ParseRes(
                config["Display:Low:Width"] ?? "1280",
                 config["Display:Low:Height"] ?? "960");
            var lowColorDepth = ParseColorDepth(config["Display:Low:Depth"] ?? "32");
            Low = new Resolution(lowWidth, lowHeight, lowColorDepth);

            (var highWidth, var highHeight) = ParseRes(
                config["Display:High:Width"] ?? "1920",
                config["Display:High:Height"] ?? "1080");
            var highColorDepth = ParseColorDepth(config["Display:High:Depth"] ?? "32");
            High = new Resolution(highWidth, highHeight, highColorDepth);
            }

        private (uint Width, uint Height) ParseRes(string w, string h)
        {
            uint width = uint.Parse(w);
            uint height = uint.Parse(h);
            return (width, height);
        }

        private uint ParseColorDepth(string config)
        {
            uint Depth = uint.Parse(config);
            if (Depth is not (16 or 24 or 30 or 32))
            {
                Logger.Log($"Depth is not set to acceptable value, it is set to: {Depth}. Setting to 32");
                return 32;
            }
            return Depth;

        }

        private static IConfigurationRoot ConfigService()
        {
            return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
        }
    }
}