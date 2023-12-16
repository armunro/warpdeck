using System.Text.RegularExpressions;

namespace COSMIC.Warpdeck.Domain.Clipboard
{
    public class ClipPattern
    {
        public string Name { get; set; }
        public string RegexPattern { get; set; }
        public string Action { get; set; }

        public string ActionParamTemplate { get; set; }


        public List<ClipSuggestion> OfferSuggestions(string? text)
        {
            List<ClipSuggestion> suggestions = new List<ClipSuggestion>();
            MatchCollection matches = new Regex(RegexPattern).Matches(text);
            foreach (Match match in matches)
            {
                suggestions.Add(new ClipSuggestion
                {
                    ActionName = Action,
                    Match = match.Value,
                    PatternName = Name,
                    ActionParameters = CompileParameters(match.Value)
                });
            }
            return suggestions;
        }

        private string CompileParameters(string match)
        {
            return ActionParamTemplate.Replace("{match}", match);
        }

        public static ClipPattern Create(string name, string pattern, string actionName, string actionParamTemplate)
        {
            return new ClipPattern
            {
                Name = name,
                RegexPattern = pattern,
                Action = actionName,
                ActionParamTemplate = actionParamTemplate
            };
        }
    }
}