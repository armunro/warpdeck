using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace COSMIC.Warpdeck.Windows.Domain{

    public class Pattern
    {
        public string Name;
        public Regex Regex;
        public Func<Pattern, Match, IEnumerable<Suggestion>> SuggestionFactory;

        public List<Suggestion> OfferSuggestions(string? text)
        {
            List<Suggestion> suggestions = new List<Suggestion>();
            MatchCollection matches = Regex.Matches(text);
            foreach (Match match in matches)
            {
                IEnumerable<Suggestion> newSuggestions = SuggestionFactory(this, match);
                foreach (Suggestion tSuggest in newSuggestions)
                {
                    tSuggest.Match = match.Value;
                    tSuggest.PatternName = Name;
                    suggestions.Add(tSuggest);
                }



            }

            return suggestions;
        }

        public static Pattern Create(string name, string pattern,
            Func<Pattern, Match, IEnumerable<Suggestion>> suggestionFactory)
        {
            return new Pattern
            {
                Name = name,
                Regex = new Regex(pattern),
                SuggestionFactory = suggestionFactory
            };
        }


    }
}