using System.Collections.Generic;
using COSMIC.Warpdeck.Domain.Button;

namespace COSMIC.Warpdeck.Windows.Plugins.Actions.Paste;

public class PasteModel : ButtonActionModel
{
    public string Text { get; set; }

    public override void MapParameters(Dictionary<string, string> parameters)
    {
        if (parameters.ContainsKey("Text"))
        {
            Text = parameters["Text"];
        }
    }
}