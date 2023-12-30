using Microsoft.AspNetCore.Mvc.RazorPages;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Managers;

namespace COSMIC.Warpdeck.Web.Pages
{
    public class TouchDevice : PageModel
    {
        private readonly DeviceManager _deviceManager;
        public DeviceModel Device;

        public TouchDevice(DeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        public void OnGet()
        {
            Device = _deviceManager.GetDevice(RouteData.Values["deviceId"].ToString());
        }
    }
}