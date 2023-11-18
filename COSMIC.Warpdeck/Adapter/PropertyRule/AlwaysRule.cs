using COSMIC.Warpdeck.Domain.Property;
using COSMIC.Warpdeck.Domain.Property.Rules;

namespace COSMIC.Warpdeck.Adapter.PropertyRule
{
    public class AlwaysRule : IPropertyRule
    {
        public bool IsMetBy(PropertyLookup properties) => true;
    }
}