using System;
using System.Drawing;
using Microsoft.AspNetCore.Mvc;
using WarpDeck.Domain.Device;
using WarpDeck.Domain.Key;
using WarpDeck.Extensions;
using WarpDeck.Helpers;
using WarpDeck.UseCase.Device;

namespace WarpDeck.Presentation.Controllers
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

        [HttpGet, Route("layer/active/icon/{keyId:int}")]
        public IActionResult RenderLiveStateIcon(string deviceId, int keyId)
        {
            Bitmap icon;

            try
            {
                KeyModel keyInput = null;
                if (_deviceManager.GetDevice(deviceId).KeyStates.IsKeyMapped(keyId))
                    keyInput = _deviceManager.GetDevice(deviceId).KeyStates[keyId];
                icon = _deviceManager.GenerateKeyIcon(keyInput, deviceId);
            }
            catch (Exception)
            {
                icon = IconHelpers.DrawBlankKeyIcon(244, 244); 
            }
            return File(icon.ToMemoryStream(), "image/png", "key.png");
        }

        [HttpGet, Route("layer/{layerId}/icon/{keyId:int}")]
        public IActionResult RenderLayerIcon(string deviceId, string layerId, int keyId)
        {
            Bitmap icon;
            try
            {
                DeviceModel device = _deviceManager.GetDevice(deviceId);
                KeyMap keyMap = device.Layers.GetLayerById(layerId).Keys;

                
                if (keyMap.ContainsKey(keyId))
                {
                    KeyModel key = keyMap[keyId];
                    icon = _deviceManager.GenerateKeyIcon(key, deviceId);
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