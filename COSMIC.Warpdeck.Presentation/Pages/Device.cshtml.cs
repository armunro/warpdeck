using Microsoft.AspNetCore.Mvc.RazorPages;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.UseCase.Device;

namespace COSMIC.Warpdeck.Presentation.Pages
{
    public class Device : PageModel
    {
        private readonly DeviceManager _deviceManager;
        public DeviceModel CurrentDevice;

        public Device(DeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        public void OnGet()
        {
            CurrentDevice = _deviceManager.GetDevice(RouteData.Values["deviceId"].ToString());
        }
    }
}