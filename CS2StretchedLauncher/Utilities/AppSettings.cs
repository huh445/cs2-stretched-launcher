namespace CS2StretchedLauncher
{
    public readonly record struct Resolution(uint Width, uint Height, uint Depth = 32);

    public sealed record class AppSettings
    {
        public Resolution Low { get; init; } = new(1280, 960, 32);
        public Resolution High { get; init; } = new(1920, 1080, 32);
    }
}