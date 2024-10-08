using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.Device.Hardware;
using COSMIC.Warpdeck.Managers;

namespace COSMIC.Warpdeck.Web.Controllers
{
    [ApiController]
    [Route("api/hardware")]
    public class HardwareController : Controller
    {
        private readonly IHardwareProvider _boardProvider;
        private readonly DeviceManager _deviceManager;

        public HardwareController( IHardwareProvider boardProvider, DeviceManager deviceManager)
        {
            _boardProvider = boardProvider;
            _deviceManager = deviceManager;
        }
        
        [HttpGet, Route("")]
        public List<HardwareInfo> Index()
        {
            return _boardProvider.ProvideAttachedHardwareInfo();
        }
        
        [HttpGet, Route("available")]
        public List<HardwareInfo> GetAvailableHardware()
        {
            var availableBoards = _boardProvider.ProvideAttachedHardwareInfo();
            var registeredIdentifiers = _deviceManager.GetAllDevices().Select(x => x.Info.HardwareId);
            return availableBoards.Where(x => !registeredIdentifiers.Contains(x.HardwareId)).ToList();
        }
    }
}