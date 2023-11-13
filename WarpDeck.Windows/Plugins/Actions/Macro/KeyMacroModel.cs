using System.Collections.Generic;
using WarpDeck.Domain.Key;

namespace WarpDeck.Windows.Plugins.Actions.Macro
{
    public class KeyMacroModelModel : ActionModelModel
    {
        public string Keys { get; set; }

        public override void MapParameters(Dictionary<string, string> parameters)
        {
            Keys = parameters["keys"];
        }
    }
}