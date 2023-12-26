using System.Collections.Generic;
using COSMIC.Warpdeck.Domain.Button;

namespace COSMIC.Warpdeck.Windows.Plugins.Actions.WebLaunch;

public class WebLaunchModel : ButtonActionModel
{
    public string Url { get; set; }

    public override void MapParameters(Dictionary<string, string> parameters)
    {
        if (parameters.ContainsKey("Url"))
        {
            Url = parameters["Url"];
        }
    }
}