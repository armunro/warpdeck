namespace Warpdeck.Infrastructure.Adapters.Yaml;

public class LayerConfig
{
    public string Name { get; set; } = string.Empty;
    public int Priority { get; set; }
    public string Color { get; set; } = "#000000";
    public string? FontColor { get; set; }
    public ConditionConfig? Condition { get; set; }
    public List<ButtonConfig> Buttons { get; set; } = new();
}
