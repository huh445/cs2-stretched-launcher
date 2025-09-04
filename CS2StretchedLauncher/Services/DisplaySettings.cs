
namespace CS2StretchedLauncher.Services
{
    internal class DisplaySettings(DisplaySettings.Resolution low, DisplaySettings.Resolution high)
    {
        public readonly record struct Resolution(uint Width, uint Height, uint Depth = 32);

        public Resolution Low { get; } = low;
        public Resolution High { get; } = high;
    }
}