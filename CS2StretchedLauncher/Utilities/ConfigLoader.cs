using System.Text.Json;

class ConfigLoader
{
    public static ConfigLoader Load(string filename)
    {
        string json = File.ReadAllText(filename);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var root = JsonSerializer.Deserialize<Dictionary<string, ConfigLoader>>(json, options);
        return root["Display"];
    }
}