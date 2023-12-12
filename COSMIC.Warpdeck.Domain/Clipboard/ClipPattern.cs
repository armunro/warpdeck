using System.Text.RegularExpressions;

namespace COSMIC.Warpdeck.Domain.Clipboard
{
    public class ClipPattern
    {
        public string Name { get; set; }
        public string RegexPattern {get; set;}
        public Func<ClipPattern, Match, IEnumerable<ClipSuggestion>> SuggestionFactory;

        public List<ClipSuggestion> OfferSuggestions(string? text)
        {
            List<ClipSuggestion> suggestions = new List<ClipSuggestion>();
            MatchCollection matches = new Regex(RegexPattern).Matches(text);
            foreach (Match match in matches)
            {
                IEnumerable<ClipSuggestion> newSuggestions = SuggestionFactory(this, match);
                foreach (ClipSuggestion tSuggest in newSuggestions)
                {
                    tSuggest.Match = match.Value;
                    tSuggest.PatternName = Name;
                    suggestions.Add(tSuggest);
                }
            }

            return suggestions;
        }

        public static ClipPattern Create(string name, string pattern,
            Func<ClipPattern, Match, IEnumerable<ClipSuggestion>> suggestionFactory)
        {
            return new ClipPattern
            {
                Name = name,
                RegexPattern = pattern,
                SuggestionFactory = suggestionFactory
            };
        }
    }
}