using Microsoft.AspNetCore.Mvc.RazorPages;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.Property.Rules;

namespace COSMIC.Warpdeck.Web.Pages
{
    public class Properties : PageModel
    {
        private readonly DeviceManager _deviceManager;
        public string DeviceId { get; set; }

        public PropertyRuleModelList PropertyRules { get; set; }

        public Properties(DeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        public void OnGet()
        {
            DeviceId = RouteData.Values["deviceId"].ToString();
            PropertyRules = _deviceManager.GetDevice(DeviceId).PropertyRules;
        }
    }
}