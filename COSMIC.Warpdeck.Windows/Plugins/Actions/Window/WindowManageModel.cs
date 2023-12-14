using System;
using System.Collections.Generic;
using COSMIC.Warpdeck.Domain.Button;

namespace COSMIC.Warpdeck.Windows.Plugins.Actions.Window
{
    public enum WindowState
    {
        Normal,
        Minimized,
        Maximized
    }

    public class WindowManageModel : ButtonActionModel
    {
        public string ProcessName { get; set; }
        public string WindowTitle { get; set; }
        public int ScreenNumber { get; set; } = -1;
        public WindowState WindowState { get; set; } = WindowState.Normal;


        public override void MapParameters(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("processName"))
            {
                ProcessName = parameters["processName"];
            }

            if (parameters.ContainsKey("screenNumber"))
            {
                ScreenNumber = Int32.Parse(parameters["screenNumber"]);
            }

            if (parameters.ContainsKey("windowTitle"))
            {
                WindowTitle = parameters["windowTitle"];
            }

            if (parameters.ContainsKey("windowState"))
            {
                WindowState = Enum.Parse<WindowState>(parameters["windowState"], true);
            }
        }
    }
}