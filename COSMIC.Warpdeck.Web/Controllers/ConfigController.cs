using Microsoft.AspNetCore.Mvc;
using COSMIC.Warpdeck.Domain.Configuration;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.UseCase.Device;

namespace COSMIC.Warpdeck.Web.Controllers
{
    [ApiController]
    [Route("api/config/device/{deviceId}")]
    public class ConfigController : Controller
    {
        private readonly DeviceManager _deviceManager;
        private readonly IDeviceWriter _deviceWriter;

        public ConfigController(DeviceManager deviceManager, IDeviceWriter deviceWriter)
        {
            _deviceManager = deviceManager;
            _deviceWriter = deviceWriter;
        }

        [HttpGet, Route("")]
        public IActionResult GetConfig(string deviceId)
        {
            return Json(_deviceManager.GetDevice(deviceId));
        }

        [HttpGet, Route("save")]
        public IActionResult SaveDeviceConfig(string deviceId)
        {
            DeviceModel device = _deviceManager.GetDevice(deviceId);
            _deviceWriter.WriteDeviceModel(device);
            return Ok("Device was saved");
        }
    }
}