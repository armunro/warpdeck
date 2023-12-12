using System.Collections.Generic;
using System.Linq;
using Autofac;
using COSMIC.Warpdeck.Domain.Key.Action;
using COSMIC.Warpdeck.Domain.Key.Action.Descriptors;
using Microsoft.AspNetCore.Mvc;

namespace COSMIC.Warpdeck.Web.Controllers
{
    [ApiController]
    public class ActionController : Controller
    {

        [HttpGet, Route("/api/action")]
        public IActionResult GetActions()
        {
            return Json(WarpDeckFrontend.Container.ComponentRegistry.Registrations
                .Where(r => typeof(KeyAction).IsAssignableFrom(r.Activator.LimitType))
                .Select(x => x.Activator.LimitType.Name));
        }
        
        
        [HttpGet, Route("/api/action/{actionName}/parameters")]
        public ActionParamDescriptorSet GetActionParameters(string actionName)
        {
            IEnumerable<IHasActionParameters> hasActions = WarpDeckFrontend.Container.Resolve<IEnumerable<IHasActionParameters>>();
            IHasActionParameters action = hasActions.FirstOrDefault(x => x.GetType().Name == actionName);
            ActionParamDescriptorSet parameters = action.SpecifyParameters();
            return parameters;
        }
    }
}