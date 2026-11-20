namespace Warpdeck.Infrastructure.Adapters.Yaml;

public class ButtonConfig
{
    public int Index { get; set; }
    public string? Color { get; set; }
    public string? FontColor { get; set; }
    public string? Caption { get; set; }
    public string? Icon { get; set; }
    public string? IconRoot { get; set; }
    public string? Theme { get; set; }
    public ActionConfig? Action { get; set; }
}
