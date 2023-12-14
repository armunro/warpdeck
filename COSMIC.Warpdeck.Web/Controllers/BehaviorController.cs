using System.Collections.Generic;
using System.Linq;
using Autofac;
using COSMIC.Warpdeck.Domain.Action;
using Microsoft.AspNetCore.Mvc;

namespace COSMIC.Warpdeck.Web.Controllers
{
    [ApiController]
    public class BehaviorController : Controller
    {
        [HttpGet, Route("api/behavior")]
        public IActionResult GetBehaviorTypeNames()
        {
            return Json(WarpDeckFrontend.Container.ComponentRegistry.Registrations
                .Where(r => typeof(ButtonBehavior).IsAssignableFrom(r.Activator.LimitType))
                .Select(x => x.Activator.LimitType.Name));
        }
        
        [HttpGet, Route("api/behavior/{behaviorType}/actions")]
        public object GetBehaviorActions(string behaviorType)
        {
            IEnumerable<IHasActions> hasActions = WarpDeckFrontend.Container.Resolve<IEnumerable<IHasActions>>();
            IHasActions behavior = hasActions.FirstOrDefault();
            return behavior.SpecifyActions();



        }
    }
}