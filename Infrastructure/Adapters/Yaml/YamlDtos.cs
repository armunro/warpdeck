namespace Warpdeck.Infrastructure.Adapters.Yaml;

public class AppConfig
{
    public string? IconRoot { get; set; }
    public List<LayerConfig> Layers { get; set; } = new();
}
