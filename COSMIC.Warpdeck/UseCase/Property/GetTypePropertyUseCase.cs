using System.Collections.Generic;
using System.Linq;
using COSMIC.Warpdeck.Domain.Property;
using COSMIC.Warpdeck.Domain.Property.Descriptors;

namespace COSMIC.Warpdeck.UseCase.Property
{
    public class GetTypePropertyUseCase
    {
        private readonly IEnumerable<IHasProperties> _typesWithProperties;

        public GetTypePropertyUseCase(IEnumerable<IHasProperties> typesWithProperties)
        {
            _typesWithProperties = typesWithProperties;
        }

        public PropertyDescriptorSet Invoke(string parentType, string typeName)
        {
            return _typesWithProperties.First(x => x.GetType().BaseType.Name == parentType &&
                                                   x.GetType().Name == typeName).SpecifyProperties();
        }
    }
}