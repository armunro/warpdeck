using Microsoft.AspNetCore.Mvc.RazorPages;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.UseCase.Device;

namespace COSMIC.Warpdeck.Presentation.Pages
{
    public class DeviceBare : PageModel
    {
        private readonly DeviceManager _deviceManager;
        public DeviceModel CurrentDevice;

        public DeviceBare(DeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        public void OnGet()
        {
            CurrentDevice = _deviceManager.GetDevice(RouteData.Values["deviceId"].ToString());
        }
    }
}