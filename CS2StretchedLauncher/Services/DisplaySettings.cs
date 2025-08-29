using CS2StretchedLauncher.Services;
using CS2StretchedLauncher.Utilities;



namespace CS2StretchedLauncher
{
    internal class DisplaySettings
    {
        public readonly record struct Resolution(uint Width, uint Height, uint Depth = 32);

        public Resolution Low { get; }
        public Resolution High { get; }

        public DisplaySettings(Resolution low, Resolution high)
        {
            Low = low;
            High = high;
        }
    }
}