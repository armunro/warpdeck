using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace COSMIC.Warpdeck.Domain.Clipboard
{
    public class ClipboardPattern
    {
        public string Name;
        public Regex Regex;
        public Func<ClipboardPattern, Match, IEnumerable<ClipboardSuggestion>> SuggestionFactory;

        public List<ClipboardSuggestion> OfferSuggestions(string? text)
        {
            List<ClipboardSuggestion> suggestions = new List<ClipboardSuggestion>();
            MatchCollection matches = Regex.Matches(text);
            foreach (Match match in matches)
            {
                IEnumerable<ClipboardSuggestion> newSuggestions = SuggestionFactory(this, match);
                foreach (ClipboardSuggestion tSuggest in newSuggestions)
                {
                    tSuggest.Match = match.Value;
                    tSuggest.PatternName = Name;
                    suggestions.Add(tSuggest);
                }
            }

            return suggestions;
        }

        public static ClipboardPattern Create(string name, string pattern,
            Func<ClipboardPattern, Match, IEnumerable<ClipboardSuggestion>> suggestionFactory)
        {
            return new ClipboardPattern
            {
                Name = name,
                Regex = new Regex(pattern),
                SuggestionFactory = suggestionFactory
            };
        }
    }
}