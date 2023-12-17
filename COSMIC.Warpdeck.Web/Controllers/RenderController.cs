using System;
using System.Drawing;
using COSMIC.Warpdeck.Domain.Button;
using Microsoft.AspNetCore.Mvc;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.Icon;
using COSMIC.Warpdeck.Managers;

namespace COSMIC.Warpdeck.Web.Controllers
{
    [ApiController]
    [Route("api/device/{deviceId}")]
    public class RenderController : Controller
    {
        private readonly DeviceManager _deviceManager;

        public RenderController(DeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        [HttpGet, Route("layer/active/icon/{keyId}")]
        public IActionResult RenderLiveStateIcon(string deviceId, string keyId)
        {
            Bitmap icon;

            try
            {
                ButtonModel buttonInput = null;
                if (_deviceManager.GetDevice(deviceId).ButtonStates.IsKeyMapped(keyId))
                    buttonInput = _deviceManager.GetDevice(deviceId).ButtonStates[keyId];
                icon = _deviceManager.GenerateKeyIcon(buttonInput, deviceId);
            }
            catch (Exception)
            {
                icon = IconHelpers.DrawBlankKeyIcon(244, 244); 
            }
            return File(icon.ToMemoryStream(), "image/png", "key.png");
        }

        [HttpGet, Route("layer/{layerId}/icon/{keyId}")]
        public IActionResult RenderLayerIcon(string deviceId, string layerId, string keyId)
        {
            Bitmap icon;
            try
            {
                DeviceModel device = _deviceManager.GetDevice(deviceId);
                ButtonMap buttonMap = device.Layers.GetLayerById(layerId).Buttons;

                
                if (buttonMap.ContainsKey(keyId))
                {
                    ButtonModel button = buttonMap[keyId];
                    icon = _deviceManager.GenerateKeyIcon(button, deviceId);
                }
                else
                {
                    icon = IconHelpers.DrawBlankKeyIcon(244, 244);
                }
            }
            catch (Exception)
            {
                icon = IconHelpers.DrawBlankKeyIcon(244, 244);
            }

            return File(icon.ToMemoryStream(), "image/png", "key.png");    
            
        }
    }
}