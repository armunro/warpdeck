using Microsoft.AspNetCore.Mvc.RazorPages;
using WarpDeck.Domain.Device;
using WarpDeck.Domain.Property.Rules;
using WarpDeck.UseCase.Device;

namespace WarpDeck.Presentation.Pages
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