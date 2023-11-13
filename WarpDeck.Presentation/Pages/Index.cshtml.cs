using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WarpDeck.Domain.Device;
using WarpDeck.UseCase.Device;

namespace WarpDeck.Presentation.Pages
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