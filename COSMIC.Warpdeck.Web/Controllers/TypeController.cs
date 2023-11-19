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
    public class TypeController : Controller
    {
        [HttpGet, Route("api/type/{typeName}")]
        public string[] GetTypeOptions(string typeName)
        {
            Type warpType = typeName switch
            {
                nameof(KeyBehavior) => typeof(KeyBehavior),
                _ => typeof(KeyBehavior)
            };

            var resolve = (IEnumerable<KeyBehavior>) WarpDeckFrontend.Container.Resolve(typeof(IEnumerable<>).MakeGenericType(warpType));
            return resolve.Select(x => x.GetType().Name).ToArray();
            
        }
    }
}