namespace COSMIC.Warpdeck.Windows.Domain
{

    public class Suggestion
    {
        public string PatternName { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Value { get; set; } = null!;
        public string Match { get; set; } = null!;
    }
}