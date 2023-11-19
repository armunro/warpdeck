using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.UseCase.Device;

namespace COSMIC.Warpdeck.Web.Pages
{
    public class Index : PageModel
    {
        private readonly DeviceManager _deviceManager;
        public DeviceModel Device { get; private set; }

        public Index(DeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        // ReSharper disable once UnusedMember.Global
        public void OnGet()
        {
            Device = _deviceManager.GetAllDevices().First();
        }
    }
}