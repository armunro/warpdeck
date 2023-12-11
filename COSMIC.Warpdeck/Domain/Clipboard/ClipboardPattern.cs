using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace COSMIC.Warpdeck.Domain.Clipboard
{
    public class ClipboardPattern
    {
        public string Name { get; set; }
        public Regex Regex;
        public Func<ClipboardPattern, Match, IEnumerable<ClipSuggestion>> SuggestionFactory;

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

        public static ClipboardPattern Create(string name, string pattern,
            Func<ClipboardPattern, Match, IEnumerable<ClipSuggestion>> suggestionFactory)
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