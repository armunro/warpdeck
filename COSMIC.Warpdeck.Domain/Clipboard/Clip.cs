namespace COSMIC.Warpdeck.Domain.Clipboard
{
    public class Clip
    {
        public DateTime Time { get; set; }
        public string Text { get; set; }
        public List<ClipSuggestion> Suggestions { get; set; } = new();
    }
}