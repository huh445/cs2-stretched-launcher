using CS2StretchedLauncher.Services;
using CS2StretchedLauncher.Utilities;



namespace CS2StretchedLauncher
{
    class DisplaySettings
    {
                public readonly record struct Resolution(uint Width, uint Height, uint Depth = 32);
    }
}