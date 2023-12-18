using COSMIC.Warpdeck.Domain.Property;
using COSMIC.Warpdeck.Domain.Property.Rules;

namespace COSMIC.Warpdeck.Adapter.PropertyRule
{
    public class PropertyEqualsRule : IPropertyRule
    {
        private readonly string _updateProperty;
        private readonly string _matches;

        public PropertyEqualsRule(string Property, string Matches)
        {
            _updateProperty = Property;
            _matches = Matches;
        }

        public bool IsMetBy(PropertyLookup properties)
        {
            if(!properties.ContainsKey(_updateProperty))
                return false;
            string tag = properties[_updateProperty];
            if (tag == null)
                return false;
            return tag == _matches;
        }
    }
}