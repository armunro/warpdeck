using System.Collections.Generic;
using COSMIC.Warpdeck.Domain.Button;

namespace COSMIC.Warpdeck.Windows.Plugins.Actions.Macro
{
    public class KeyMacroModel : ButtonActionModel
    {
        public string Keys { get; set; }

        public override void MapParameters(Dictionary<string, string> parameters)
        {
            Keys = parameters["keys"];
        }
    }
}