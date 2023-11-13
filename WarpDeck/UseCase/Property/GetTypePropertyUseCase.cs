using System.Collections.Generic;
using System.Linq;
using WarpDeck.Domain.Property;
using WarpDeck.Domain.Property.Descriptors;

namespace WarpDeck.UseCase.Property
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