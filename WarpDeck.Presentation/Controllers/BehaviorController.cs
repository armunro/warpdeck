using System.Collections.Generic;
using System.Linq;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using WarpDeck.Domain.Key;
using WarpDeck.Domain.Key.Action;
using WarpDeck.Domain.Key.Behavior;

namespace WarpDeck.Presentation.Controllers
{
    [ApiController]
    public class BehaviorController : Controller
    {
        [HttpGet, Route("api/behavior")]
        public IActionResult GetBehaviorTypeNames()
        {
            return Json(WarpDeckFrontend.Container.ComponentRegistry.Registrations
                .Where(r => typeof(KeyBehavior).IsAssignableFrom(r.Activator.LimitType))
                .Select(x => x.Activator.LimitType.Name));
        }
        
        [HttpGet, Route("api/behavior/{behaviorType}/actions")]
        public object GetBehaviorActions(string behaviorType)
        {
            IEnumerable<IHasActions> hasActions = WarpDeckFrontend.Container.Resolve<IEnumerable<IHasActions>>();
            IHasActions behavior = hasActions.FirstOrDefault(x => x.GetType().Name == behaviorType);
            return behavior.SpecifyActions();



        }
    }
}