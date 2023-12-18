using Autofac;
using COSMIC.Warpdeck.Domain.Action;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Managers;
using Microsoft.AspNetCore.Mvc;

namespace COSMIC.Warpdeck.Web.Controllers
{
    [ApiController]
    public class TriggerController : Controller
    {
        [HttpPost, Route("api/trigger/{deviceName}/{keyId}/press")]
        public string KeyPress(string deviceName, string keyId)
        {
            WarpdeckAppContext.Container.Resolve<DeviceManager>().TriggerButtonAction(deviceName, keyId, "press");
            return "pressed";
        }

        [HttpPost, Route("api/trigger/{deviceName}/{keyId}/hold")]
        public string KeyHold(string deviceName, string keyId)
        {
            WarpdeckAppContext.Container.Resolve<DeviceManager>().TriggerButtonAction(deviceName, keyId, "hold");
            return "held";
        }
        
        [HttpPost, Route("api/trigger/action")]
        public string TriggerAction(string actionName, [FromBody] ActionModel action)
        {
            WarpdeckAppContext.Container.Resolve<DeviceManager>().TriggerAction(action);
            return "held";
        }
        
    }
}