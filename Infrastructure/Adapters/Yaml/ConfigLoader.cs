using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Warpdeck.Infrastructure.Adapters.Yaml;

public static class ConfigLoader
{
    public static AppConfig Load(string path)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        using var reader = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
        string yaml = reader.ReadToEnd();
        var config = deserializer.Deserialize<AppConfig>(yaml);
        return config ?? new AppConfig();
    }
}
