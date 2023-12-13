namespace COSMIC.Warpdeck.Domain.Clipboard
{

    public class ClipSuggestion
    {
        public string PatternName { get; set; } = null!;
        public string ActionName { get; set; } = null!;
        public string ActionParameters { get; set; } = null!;
        public string Match { get; set; } = null!;
    }
}