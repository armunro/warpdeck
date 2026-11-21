namespace Warpdeck.Infrastructure.Adapters.Yaml;

public class AppConfig
{
    public string? IconRoot { get; set; }
    public byte? Brightness { get; set; } = 100;
    public List<LayerConfig> Layers { get; set; } = new();
}
