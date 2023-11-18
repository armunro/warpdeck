using System.Collections.Generic;
using System.Linq;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using COSMIC.Warpdeck.Domain.Property;
using COSMIC.Warpdeck.Domain.Property.Descriptors;
using COSMIC.Warpdeck.Presentation.Controllers.Models;
using COSMIC.Warpdeck.UseCase.Property;

namespace COSMIC.Warpdeck.Presentation.Controllers
{
    [ApiController]
    [Route("api")]
    public class PropertyController : Controller
    {
        private readonly GetTypePropertyUseCase _getTypePropertyUseCase;

        public PropertyController(GetTypePropertyUseCase getTypePropertyUseCase)
        {
            _getTypePropertyUseCase = getTypePropertyUseCase;
        }

        // GET
        [HttpGet]
        [Route("property")]
        public IEnumerable<TypePropertiesResponseModel> Index()
        {
            
            IEnumerable<IHasProperties> hasProps = WarpDeckFrontend.Container.Resolve<IEnumerable<IHasProperties>>();

            return hasProps.Select(x => new TypePropertiesResponseModel()
            {
                TypeName = x.GetType().Name,
                Properties = x.SpecifyProperties(),
               
                
            });
        }
        
        [HttpGet]
        [Route("property/{parentTypeName}/{typeName}")]
        public PropertyDescriptorSet Index(string parentTypeName, string typeName)
        {

            return _getTypePropertyUseCase.Invoke(parentTypeName, typeName);

        }
    }
}