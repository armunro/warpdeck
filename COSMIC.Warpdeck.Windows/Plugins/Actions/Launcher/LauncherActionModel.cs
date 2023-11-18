using System.Collections.Generic;
using COSMIC.Warpdeck.Domain.Key;

namespace COSMIC.Warpdeck.Windows.Plugins.Actions.Launcher
{
    public class LauncherActionModelModel : ActionModelModel
    {
        public string AppPath { get; set; }
        public string ProcessName { get; set; }
        public string Arguments { get; set; }


        public override void MapParameters(Dictionary<string, string> parameters)
        {
            AppPath = parameters["path"];
            ProcessName = parameters["processName"];

            if (parameters.ContainsKey("arguments"))
                Arguments = parameters["arguments"];
        }
    }
}