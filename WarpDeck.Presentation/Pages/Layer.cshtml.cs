using Microsoft.AspNetCore.Mvc.RazorPages;
using WarpDeck.Domain.Device;
using WarpDeck.Domain.Key;
using WarpDeck.UseCase.Device;

namespace WarpDeck.Presentation.Pages
{
    public class Layer : PageModel
    {
        private readonly DeviceManager _deviceManager;
        public KeyMap LayerKeys { get; set; }
        public string LayerId { get; set; }
        public string DeviceId { get; set; }

        
        public Layer(DeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        // ReSharper disable once UnusedMember.Global
        public void OnGet()
        {
            LayerId = RouteData.Values["layerId"]?.ToString();
            DeviceId = RouteData.Values["deviceId"]?.ToString();

            LayerKeys = _deviceManager.GetDevice(DeviceId).Layers[LayerId].Keys;
        }
    }
}