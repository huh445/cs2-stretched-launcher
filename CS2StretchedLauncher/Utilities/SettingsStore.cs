using System;
using System.IO;
using System.Text.Json;

namespace CS2StretchedLauncher.Utilities
{
    public interface ISettingsStore
    {
        AppSettings Load();
        void Save(AppSettings settings);
        string Path { get; }
    }

    public sealed class JsonSettingsStore : ISettingsStore
    {
        private readonly string _path;
        private static readonly JsonSerializerOptions _json = new() { WriteIndented = true };

        public JsonSettingsStore(string appName = "CS2StretchedLauncher")
        {
            var dir = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                appName);
            Directory.CreateDirectory(dir);
            _path = System.IO.Path.Combine(dir, "settings.json");
        }

        public string Path => _path;

        public AppSettings Load()
        {
            if (!File.Exists(_path)) return new AppSettings();
            var json = File.ReadAllText(_path);
            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }

        public void Save(AppSettings settings)
        {
            var json = JsonSerializer.Serialize(settings, _json);
            File.WriteAllText(_path, json);
        }
    }
}