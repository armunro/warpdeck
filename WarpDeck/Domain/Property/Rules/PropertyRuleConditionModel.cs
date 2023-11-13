using System.Collections.Generic;
using System.Linq;

namespace WarpDeck.Domain.Property.Rules
{
    public class PropertyRuleConditionModel
    {
        public string Type { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new();

        public override string ToString()
        {
            return $"{Type} : {string.Join("+",  Parameters.Select(x => $"{x.Key} = {x.Value}"))}";
        }
    }
}