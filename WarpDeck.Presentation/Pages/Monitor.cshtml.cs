using Microsoft.AspNetCore.Mvc.RazorPages;
using WarpDeck.Domain.Device;
using WarpDeck.Domain.Monitor.Rules;
using WarpDeck.UseCase.Device;

namespace WarpDeck.Presentation.Pages
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