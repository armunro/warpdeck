using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.Hardware;
using COSMIC.Warpdeck.Domain.Layer;
using COSMIC.Warpdeck.Presentation.Controllers.Models;
using COSMIC.Warpdeck.UseCase.Device;
using COSMIC.Warpdeck.UseCase.DeviceLayer;

namespace COSMIC.Warpdeck.Presentation.Controllers
{
    [ApiController]
    [Route("api")]
    public class DeviceController : Controller
    {
        private readonly DeviceManager _deviceManager;
        private readonly ActivateDeviceLayerUseCase _activateLayer;
        private readonly UpdateDeviceUseCase _updateDevice;
        private readonly CreateDeviceUseCase _createDevice;

        public DeviceController(DeviceManager deviceManager,
            ActivateDeviceLayerUseCase activateLayer,
            UpdateDeviceUseCase updateDevice, CreateDeviceUseCase createDevice)
        {
            _deviceManager = deviceManager;
            _activateLayer = activateLayer;
            _updateDevice = updateDevice;
            _createDevice = createDevice;
        }

        // GET
        [HttpGet, Route("device")]
        public DeviceResponseModel[] AllDevices()
        {
            //TODO: should return an array
            return new[] { CreateSummaryModel(_deviceManager.GetAllDevices().First()) };
        }


        [HttpGet, Route("device/{deviceId}")]
        public DeviceResponseModel DeviceById(string deviceId)
        {
            DeviceResponseModel summaryModel =
                CreateSummaryModel(_deviceManager.GetDevice(deviceId));
            summaryModel.Layers = CreateLayerSummaryModels(_deviceManager.GetDevice(deviceId).Layers.Values, deviceId);
            return summaryModel;
        }
        
        [HttpPut, Route("device/{deviceId}")]
        public IActionResult UpdateDeviceById(string deviceId, [FromBody] DeviceModel device)
        {
            _updateDevice.Invoke(deviceId, device);
            return Ok("Device Updated");
        }

        [HttpPost, Route("device/{deviceId}")]
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
            } );
            return Ok("Board created");
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


        //TODO: This should get moved to devicelayercontroller
        [HttpGet, Route("[controller]/{deviceId}/drawLayer/{layerId}")]
        public void DrawLayer(string deviceId, string layerId)
        {
            _activateLayer.Invoke(deviceId, layerId);
        }
    }
}