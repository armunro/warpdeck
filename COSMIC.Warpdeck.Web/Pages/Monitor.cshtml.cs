using Microsoft.AspNetCore.Mvc.RazorPages;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.Monitor.Rules;
using COSMIC.Warpdeck.Managers;

namespace COSMIC.Warpdeck.Web.Pages
{
    public class Monitor : PageModel
    {
        private readonly DeviceManager _deviceManager;
        public string DeviceId { get; set; }

        public MonitorRuleList MonitorRules { get; set; }

        public Monitor(DeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        public void OnGet()
        {
            DeviceId = RouteData.Values["deviceId"].ToString();
            MonitorRules = _deviceManager.GetDevice(DeviceId).MonitorRules;
        }
    }
}