using COSMIC.Warpdeck.Domain.Button;
using Microsoft.AspNetCore.Mvc.RazorPages;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Managers;

namespace COSMIC.Warpdeck.Web.Pages
{
    public class Layer : PageModel
    {
        private readonly DeviceManager _deviceManager;
        public ButtonMap LayerButtons { get; set; }
        public string LayerId { get; set; }
        public string DeviceId { get; set; }
        public DeviceModel DeviceModel { get; set; } 

        
        public Layer(DeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        // ReSharper disable once UnusedMember.Global
        public void OnGet()
        {
            LayerId = RouteData.Values["layerId"]?.ToString();
            DeviceId = RouteData.Values["deviceId"]?.ToString();
            DeviceModel = _deviceManager.GetDevice(DeviceId);
            
            LayerButtons = DeviceModel.Layers[LayerId].Buttons;
        }
    }
}