using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.Key;
using COSMIC.Warpdeck.Domain.Key.Action;
using COSMIC.Warpdeck.Domain.Layer;
using COSMIC.Warpdeck.UseCase.Layer;
using COSMIC.Warpdeck.Web.Controllers.Models;

namespace COSMIC.Warpdeck.Web.Controllers
{
    [ApiController]
    public class DeviceLayerController : Controller
    {
        private readonly DeviceManager _deviceManager;
        private readonly NewLayerUseCase _newLayerUseCase;

        public DeviceLayerController(
            DeviceManager deviceManager,
            NewLayerUseCase newLayerUseCase)
        {
            _deviceManager = deviceManager;
            _newLayerUseCase = newLayerUseCase;
        }

        [HttpGet, Route("api/device/{deviceId}/layer")]
        public DeviceResponseModel DeviceLayersByDeviceId(string deviceId)
        {
            DeviceResponseModel summaryModel =
                CreateSummaryModel(_deviceManager.GetDevice(deviceId));
            summaryModel.Layers =
                CreateLayerSummaryModels(
                    _deviceManager.GetAllDevices().First().Layers.Values, deviceId);
            return summaryModel;
        }

        [HttpGet, Route(" api/device/{deviceId}/layer/{layerId}")]
        public LayerResponseModel DeviceLayerByDeviceAndLayerId(string deviceId, string layerId)
        {
            LayerResponseModel summaryModel =
                CreateLayerSummaryModel(_deviceManager.GetAllDevices().First().DeviceId,
                    layerId);
            summaryModel.Keys = CreateKeySummaryModels(deviceId, layerId,
                _deviceManager.GetAllDevices().First().Layers.GetLayerById(layerId)
                    .Keys);

            return summaryModel;
        }

        [HttpPost, Route("api/device/{deviceId}/layer")]
        public string NewDeviceLayer(string deviceId, [FromBody] LayerModel layer)
        {
            return _newLayerUseCase.Invoke(deviceId, layer.LayerId);
        }

        private static KeyResponseModel[] CreateKeySummaryModels(string deviceId, string layerId, KeyMap keys)
        {
            return keys.Select(x => new KeyResponseModel
            {
                Uri = $"/device/{deviceId}/layer/{layerId}/key/{x.Key}",
                IconUri = $"/render/layer/{layerId}/icon/{x.Key}",
                KeyId = x.Key,
                Behavior = new BehaviorResponseModel
                {
                    BehaviorId = x.Value.Behavior.Type,
                    Actions = CreateActionSummaries(x.Value.Behavior.Actions)
                }
            }).ToArray();
        }

        private static List<string> CreateActionSummaries(Dictionary<string, ActionModel> behaviorActions)
        {
            return behaviorActions.Select(x => x.Value.Type).ToList();
        }

        private static DeviceResponseModel CreateSummaryModel(DeviceModel device)
        {
            return new DeviceResponseModel
            {
                DeviceId = device.DeviceId,
                Uri = $"/device/{device.DeviceId}"
            };
        }

        private static LayerResponseModel[] CreateLayerSummaryModels(IEnumerable<LayerModel> layers, string deviceId)
        {
            return layers.Select(x => CreateLayerSummaryModel(deviceId, x.LayerId)).ToArray();
        }

        private static LayerResponseModel CreateLayerSummaryModel(string deviceId, string layerId)
        {
            return new LayerResponseModel
            {
                Uri = $"/device/{deviceId}/layer/{layerId}"
            };
        }
    }
}