using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Microsoft.AspNetCore.Mvc;

namespace COSMIC.Warpdeck.Web.Controllers
{
    [ApiController]
    public class TypeController : Controller
    {
        [HttpGet, Route("api/type/{typeName}")]
        public string[] GetTypeOptions(string typeName)
        {
            Type warpType = typeName switch
            {
                nameof(ButtonBehavior) => typeof(ButtonBehavior),
                _ => typeof(ButtonBehavior)
            };

            var resolve = (IEnumerable<ButtonBehavior>) WarpDeckFrontend.Container.Resolve(typeof(IEnumerable<>).MakeGenericType(warpType));
            return resolve.Select(x => x.GetType().Name).ToArray();
            
        }
    }
}