using System.Collections.Generic;
using COSMIC.Warpdeck.Domain.Action;
using COSMIC.Warpdeck.Managers;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COSMIC.Warpdeck.Web.Pages;

public class DeviceActions : PageModel
{
    private readonly DeviceManager _deviceManager;
    public string DeviceId { get; set; }
    public Dictionary<string, ActionModel> Actions { get; set; }
    
    public DeviceActions(DeviceManager deviceManager)
    {
        _deviceManager = deviceManager;
    }
    public void OnGet()
    {
        DeviceId = RouteData.Values["deviceId"].ToString();
        Actions = _deviceManager.GetDevice(DeviceId).ActionsCombined;
    }
}