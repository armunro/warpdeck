using System.Text.RegularExpressions;

namespace COSMIC.Warpdeck.Domain.Clipboard
{
    public class ClipPattern
    {
        public string Name { get; set; }
        public Regex Regex {get; set;}
        public Func<ClipPattern, Match, IEnumerable<ClipSuggestion>> SuggestionFactory;

        public List<ClipSuggestion> OfferSuggestions(string? text)
        {
            List<ClipSuggestion> suggestions = new List<ClipSuggestion>();
            MatchCollection matches = Regex.Matches(text);
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
                Regex = new Regex(pattern),
                SuggestionFactory = suggestionFactory
            };
        }
    }
}