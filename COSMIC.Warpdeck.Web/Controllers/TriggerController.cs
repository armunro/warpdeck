using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using COSMIC.Warpdeck.Domain.Key;
using COSMIC.Warpdeck.Domain.Key.Behavior;

namespace COSMIC.Warpdeck.Web.Controllers
{
    [ApiController]
    public class TriggerController : Controller
    {
        [HttpPost, Route("api/trigger/{deviceName}/{keyId}/press")]
        public string KeyPress(string deviceName, int keyId)
        {


            return "pressed";
        }
        
        [HttpPost, Route("api/trigger/{deviceName}/{keyId}/hold")]
        public string KeyHold(string deviceName, int keyId)
        {

            return "held";
        }
    }
}