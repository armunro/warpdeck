using System;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WarpDeck.Domain.Device;
using WarpDeck.Domain.Key;
using WarpDeck.Domain.Layer;
using WarpDeck.UseCase.Device;
using WarpDeck.UseCase.DeviceLayer;
using WarpDeck.UseCase.Key;

namespace WarpDeck.Presentation.Controllers
{
    [ApiController]
    [Route("api/device/{deviceId}/layer/{layerId}/key")]
    public class DeviceLayerKeyController : Controller
    {
        private readonly DeviceManager _deviceManager;
        private readonly CreateDeviceLayerKeyUseCase _createDeviceLayerKeyUseCase;
        private readonly MoveKeyUseCase _moveKeyUseCase;
        private readonly DuplicateKeyUseCase _duplicateKeyUseCase;
        private readonly RedrawDeviceLayersUseCase _redrawDeviceLayersUseCase;

        public DeviceLayerKeyController(DeviceManager deviceManager,
            CreateDeviceLayerKeyUseCase createDeviceLayerKeyUseCase,
            MoveKeyUseCase moveKeyUseCase,
            DuplicateKeyUseCase duplicateKeyUseCase,
            RedrawDeviceLayersUseCase redrawDeviceLayersUseCase)
        {
            _deviceManager = deviceManager;
            _createDeviceLayerKeyUseCase = createDeviceLayerKeyUseCase;
            _moveKeyUseCase = moveKeyUseCase;
            _duplicateKeyUseCase = duplicateKeyUseCase;
            _redrawDeviceLayersUseCase = redrawDeviceLayersUseCase;
        }

        [HttpPost]
        public IActionResult CreateLayerKey(string deviceId, string layerId,
            [FromBody] CreateLayerKeyRequestModel model)
        {
            _createDeviceLayerKeyUseCase.Invoke(deviceId, layerId, model);
            return Accepted();
        }

        [HttpGet, Route("")]
        public IActionResult GetLayerKeys(string deviceId, string layerId)
        {
            if (layerId == "active")
                return Json(_deviceManager.GetDevice(deviceId).KeyStates);
            return Json(_deviceManager.GetDevice(deviceId).Layers[layerId].Keys);
        }

        [HttpGet, Route("{keyId:int}")]
        public IActionResult GetLayerKey(string deviceId, string layerId, int keyId)
        {
            DeviceModel device;
            try
            {
                device = _deviceManager.GetDevice(deviceId);
            }
            catch (Exception)
            {
                return NotFound();
            }

            if (layerId == "active")
            {
                if (!device.KeyStates.IsKeyMapped(keyId))
                    return NotFound();
                return Json(device.KeyStates[keyId]);
            }
            else
            {
                if (!device.Layers[layerId].Keys.IsKeyMapped(keyId))
                    return NotFound();
                return Json(_deviceManager.GetDevice(deviceId).Layers[layerId].Keys[keyId],
                    new JsonSerializerOptions() { WriteIndented = true });    
            }
                
            
        }

        [HttpPut, Route("{keyId:int}")]
        public IActionResult SetLayerKey(string deviceId, string layerId, int keyId, [FromBody] KeyModel updatedKey)
        {
            _deviceManager.GetDevice(deviceId).Layers[layerId].Keys[keyId] = updatedKey;
            _deviceManager.GenerateKeyIcon(updatedKey, deviceId, true);
            _redrawDeviceLayersUseCase.Invoke(deviceId);
            return Json(_deviceManager.GetDevice(deviceId).Layers[layerId].Keys[keyId]);
        }

        [HttpGet, Route("{keyId:int}/move/{newKeyId:int}")]
        public IActionResult MoveLayerKey(string deviceId, string layerId, int keyId, int newKeyId)
        {
            _moveKeyUseCase.Invoke(deviceId, layerId, keyId, newKeyId);
            _redrawDeviceLayersUseCase.Invoke(deviceId);
            return Ok();
        }

        [HttpGet, Route("{keyId:int}/copy/{newKeyId:int}")]
        public IActionResult CopyLayerKey(string deviceId, string layerId, int keyId, int newKeyId)
        {
            _duplicateKeyUseCase.Invoke(deviceId, layerId, keyId, newKeyId);
            _redrawDeviceLayersUseCase.Invoke(deviceId);
            return Ok();
        }

        [HttpDelete, Route("{keyId:int}")]
        public IActionResult DeleteLayerKey(string deviceId, string layerId, int keyId)
        {
            LayerModel layer = _deviceManager.GetDevice(deviceId).Layers[layerId];
            layer.Keys.Remove(keyId);
            return Ok();
        }
    }
}