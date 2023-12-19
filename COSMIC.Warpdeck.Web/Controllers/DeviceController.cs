using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Autofac;
using COSMIC.Warpdeck.Domain.Action;
using COSMIC.Warpdeck.Domain.Button;
using Microsoft.AspNetCore.Mvc;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.Device.Hardware;
using COSMIC.Warpdeck.Domain.Layer;
using COSMIC.Warpdeck.Managers;
using COSMIC.Warpdeck.UseCase.Device;
using COSMIC.Warpdeck.UseCase.DeviceLayer;
using COSMIC.Warpdeck.UseCase.Key;
using COSMIC.Warpdeck.UseCase.Layer;
using COSMIC.Warpdeck.Web.Controllers.Models;

namespace COSMIC.Warpdeck.Web.Controllers
{
    [ApiController]
    [Route("api/device")]
    public class DeviceController : Controller
    {
        private readonly DeviceManager _deviceManager;
        private readonly ActivateDeviceLayerUseCase _activateLayer;
        private readonly UpdateDeviceUseCase _updateDevice;
        private readonly CreateDeviceUseCase _createDevice;
        private readonly NewLayerUseCase _newLayerUseCase;
        private readonly CreateDeviceLayerKeyUseCase _createDeviceLayerKeyUseCase;
        private readonly MoveKeyUseCase _moveKeyUseCase;
        private readonly DuplicateKeyUseCase _duplicateKeyUseCase;
        private readonly RedrawDeviceLayersUseCase _redrawDeviceLayersUseCase;

        public DeviceController(DeviceManager deviceManager,
            ActivateDeviceLayerUseCase activateLayer,
            UpdateDeviceUseCase updateDevice, CreateDeviceUseCase createDevice, NewLayerUseCase newLayerUseCase,
            CreateDeviceLayerKeyUseCase createDeviceLayerKeyUseCase, MoveKeyUseCase moveKeyUseCase,
            DuplicateKeyUseCase duplicateKeyUseCase, RedrawDeviceLayersUseCase redrawDeviceLayersUseCase)
        {
            _deviceManager = deviceManager;
            _activateLayer = activateLayer;
            _updateDevice = updateDevice;
            _createDevice = createDevice;
            _newLayerUseCase = newLayerUseCase;
            _createDeviceLayerKeyUseCase = createDeviceLayerKeyUseCase;
            _moveKeyUseCase = moveKeyUseCase;
            _duplicateKeyUseCase = duplicateKeyUseCase;
            _redrawDeviceLayersUseCase = redrawDeviceLayersUseCase;
        }

        [HttpGet, Route("")]
        public DeviceResponseModel[] AllDevices()
        {
            return _deviceManager.GetAllDevices().Select(CreateSummaryModel).ToArray();
        }


        [HttpGet, Route("{deviceId}")]
        public DeviceResponseModel DeviceById(string deviceId)
        {
            DeviceResponseModel summaryModel =
                CreateSummaryModel(_deviceManager.GetDevice(deviceId));
            summaryModel.Layers = CreateLayerSummaryModels(_deviceManager.GetDevice(deviceId).Layers.Values, deviceId);
            return summaryModel;
        }

        [HttpPut, Route("{deviceId}")]
        public IActionResult UpdateDeviceById(string deviceId, [FromBody] DeviceModel device)
        {
            _updateDevice.Invoke(deviceId, device);
            return Ok("Device Updated");
        }

        [HttpPost, Route("{deviceId}")]
        public IActionResult CreateDevice(string deviceId, [FromBody] HardwareInfo boardInfo)
        {
            _createDevice.Invoke(deviceId, new DeviceModel()
            {
                Info = new DeviceInfo()
                {
                    Columns = 8,
                    Rows = 4,
                    HardwareId = boardInfo.HardwareId
                }
            });
            return Ok("Board created");
        }


        [HttpGet, Route("{deviceId}/layer")]
        public DeviceResponseModel DeviceLayersByDeviceId(string deviceId)
        {
            DeviceResponseModel summaryModel =
                CreateSummaryModel(_deviceManager.GetDevice(deviceId));
            summaryModel.Layers =
                CreateLayerSummaryModels(
                    _deviceManager.GetDevice(deviceId).Layers.Values, deviceId);
            return summaryModel;
        }

        [HttpGet, Route("{deviceId}/layer/{layerId}")]
        public LayerResponseModel DeviceLayerByDeviceAndLayerId(string deviceId, string layerId)
        {
            LayerResponseModel summaryModel = CreateLayerSummaryModel(deviceId, layerId);
            summaryModel.Keys = CreateKeySummaryModels(deviceId, layerId,
                _deviceManager.GetDevice(deviceId).Layers.GetLayerById(layerId).Buttons);

            return summaryModel;
        }

        [HttpPost, Route("{deviceId}/layer")]
        public string NewDeviceLayer(string deviceId, [FromBody] LayerModel layer)
        {
            return _newLayerUseCase.Invoke(deviceId, layer.LayerId);
        }
        


        [HttpGet, Route("{deviceId}/layer/{layerId}/key")]
        public IActionResult GetLayerKeys(string deviceId, string layerId)
        {
            if (layerId == "active")
                return Json(_deviceManager.GetDevice(deviceId).ButtonStates);
            return Json(_deviceManager.GetDevice(deviceId).Layers[layerId].Buttons);
        }

        [HttpGet, Route("{deviceId}/layer/{layerId}/key/{keyId}")]
        public IActionResult GetLayerKey(string deviceId, string layerId, string keyId)
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
                if (!device.ButtonStates.IsKeyMapped(keyId))
                    return NotFound();
                return Json(device.ButtonStates[keyId]);
            }
            else
            {
                if (!device.Layers[layerId].Buttons.IsKeyMapped(keyId))
                    return NotFound();
                return Json(_deviceManager.GetDevice(deviceId).Layers[layerId].Buttons[keyId],
                    new JsonSerializerOptions() { WriteIndented = true });    
            }
        }
        
        [HttpPost, Route("{deviceId}/layer/{layerId}/key")]
        public IActionResult CreateLayerKey(string deviceId, string layerId,
            [FromBody] CreateLayerButtonRequestModel model)
        {
            _createDeviceLayerKeyUseCase.Invoke(deviceId, layerId, model);
            return Accepted();
        }

        [HttpPut, Route("{deviceId}/layer/{layerId}/key")]
        public IActionResult SetLayerKey(string deviceId, string layerId, string keyId, [FromBody] ButtonModel updatedButton)
        {
            _deviceManager.GetDevice(deviceId).Layers[layerId].Buttons[keyId] = updatedButton;
            _deviceManager.GenerateKeyIcon(updatedButton, deviceId, true);
            _redrawDeviceLayersUseCase.Invoke(deviceId);
            return Json(_deviceManager.GetDevice(deviceId).Layers[layerId].Buttons[keyId]);
        }

        [HttpGet, Route("{deviceId}/layer/{layerId}/key/{keyId}/move/{newKeyId}")]
        public IActionResult MoveLayerKey(string deviceId, string layerId, string keyId, string newKeyId)
        {
            _moveKeyUseCase.Invoke(deviceId, layerId, keyId, newKeyId);
            _redrawDeviceLayersUseCase.Invoke(deviceId);
            return Ok();
        }

        [HttpGet, Route("{deviceId}/layer/{layerId}/key/{keyId}/copy/{newKeyId}")]
        public IActionResult CopyLayerKey(string deviceId, string layerId, string keyId, string newKeyId)
        {
            _duplicateKeyUseCase.Invoke(deviceId, layerId, keyId, newKeyId);
            _redrawDeviceLayersUseCase.Invoke(deviceId);
            return Ok();
        }

        [HttpDelete, Route("{deviceId}/layer/{layerId}/key/{keyId}")]
        public IActionResult DeleteLayerKey(string deviceId, string layerId, string keyId)
        {
            LayerModel layer = _deviceManager.GetDevice(deviceId).Layers[layerId];
            layer.Buttons.Remove(keyId);
            return Ok();
        }
        
        [HttpPost, Route("{deviceId}/keys/{keyId}/press")]
        public string KeyPress(string deviceId, string keyId)
        {
            WarpdeckAppContext.Container.Resolve<DeviceManager>().TriggerButtonAction(deviceId, keyId, "Press");
            return "pressed";
        }

        [HttpPost, Route("{deviceId}/keys/{keyId}/hold")]
        public string KeyHold(string deviceId, string keyId)
        {
            WarpdeckAppContext.Container.Resolve<DeviceManager>().TriggerButtonAction(deviceId, keyId, "Hold");
            return "held";
        }
        


        private static KeyResponseModel[] CreateKeySummaryModels(string deviceId, string layerId, ButtonMap buttons)
        {
            return buttons.Select(x => new KeyResponseModel
            {
                Uri = $"/device/{deviceId}/layer/{layerId}/key/{x.Key}",
                IconUri = $"/render/layer/{layerId}/icon/{x.Key}",
                KeyId = x.Key,
                Behavior = new BehaviorResponseModel
                {
                    BehaviorId = "PushAndHold",
                    Actions = CreateActionSummaries(x.Value.Actions)
                }
            }).ToArray();
        }

        private static List<string> CreateActionSummaries(Dictionary<string, ActionModel> behaviorActions)
        {
            return behaviorActions.Select(x => x.Value.Type).ToList();
        }


        #region Device Summarizers

        private static DeviceResponseModel CreateSummaryModel(DeviceModel device)
        {
            return new DeviceResponseModel
            {
                DeviceId = device.DeviceId,
                Uri = $"/device/{device.DeviceId}"
            };
        }

        private static LayerResponseModel CreateLayerSummaryModel(string deviceId, string layerId)
        {
            return new LayerResponseModel
            {
                Uri = $"/device/{deviceId}/layer/{layerId}"
            };
        }

        private static LayerResponseModel[] CreateLayerSummaryModels(IEnumerable<LayerModel> layers, string deviceId)
        {
            return layers.Select(x => CreateLayerSummaryModel(deviceId, x.LayerId)).ToArray();
        }

        #endregion


        [HttpGet, Route("layer/{layerId}/draw")]
        public void DrawLayer(string deviceId, string layerId)
        {
            _activateLayer.Invoke(deviceId, layerId);
        }
    }
}