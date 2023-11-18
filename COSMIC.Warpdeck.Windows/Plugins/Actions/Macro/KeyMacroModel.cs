using System.Collections.Generic;
using COSMIC.Warpdeck.Domain.Key;

namespace COSMIC.Warpdeck.Windows.Plugins.Actions.Macro
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