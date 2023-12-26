using System;
using System.Collections.Generic;
using COSMIC.Warpdeck.Domain.Action;
using COSMIC.Warpdeck.Domain.Action.Descriptors;
using COSMIC.Warpdeck.Domain.Button;
using COSMIC.Warpdeck.Windows.Plugins.Actions.Window;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace COSMIC.Warpdeck.Windows.Plugins.Actions.WebLaunch;

public class WebLaunchAction : ButtonAction<WebLaunchModel>, IHasActionParameters
{
    public WebLaunchAction()
    {
        //Default ctor needed for IHasActionsParameters resolution
    }
    
    public WebLaunchAction(Dictionary<string, string> parameters) : base(parameters)
    {
    }
    public override void StartAction(ActionModel actionModel)
    {
        OpenBrowser(Model.Url);
    }

    public ActionParamDescriptorSet SpecifyParameters() =>
        new ActionParamDescriptorSet().Parameter("Url", "Web URL",
            "The URL to open in a new browser tab/window");

    public void OpenBrowser(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
            else
                throw;
        }
    }
}