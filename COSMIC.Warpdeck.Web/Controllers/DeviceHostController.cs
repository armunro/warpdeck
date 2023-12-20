using System.Collections.Generic;
using System.Linq;
using Autofac;
using COSMIC.Warpdeck.Domain.Action;
using COSMIC.Warpdeck.Domain.Action.Descriptors;
using COSMIC.Warpdeck.Domain.Button;
using COSMIC.Warpdeck.Managers;
using Microsoft.AspNetCore.Mvc;

namespace COSMIC.Warpdeck.Web.Controllers
{
    [ApiController]
    public class DeviceHostController : Controller
    {

        [HttpGet, Route("/api/devicehost/{deviceId}/start")]
        public void StartDeviceHost (string deviceId)
        {
           WarpdeckAppContext.Container.Resolve<DeviceHostManager>().StartDeviceHost(deviceId);
        }
        
        
        
    }
}