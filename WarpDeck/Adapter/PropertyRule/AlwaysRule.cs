using WarpDeck.Domain.Property;
using WarpDeck.Domain.Property.Rules;

namespace WarpDeck.Adapter.PropertyRule
{
    public class AlwaysRule : IPropertyRule
    {
        public bool IsMetBy(PropertyLookup properties) => true;
    }
}