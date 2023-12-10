namespace COSMIC.Warpdeck.Domain.Clipboard
{

    public class ClipboardSuggestion
    {
        public string PatternName { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Value { get; set; } = null!;
        public string Match { get; set; } = null!;
    }
}