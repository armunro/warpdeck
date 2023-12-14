using COSMIC.Warpdeck.Domain.Property;
using COSMIC.Warpdeck.Domain.Property.Rules;

namespace COSMIC.Warpdeck.Adapter.PropertyRule
{
    public class PropertyEqualsRule : IPropertyRule
    {
        private readonly string _sourceTagName;
        private readonly string _matches;

        public PropertyEqualsRule(string sourceTagName, string matches)
        {
            _sourceTagName = sourceTagName;
            _matches = matches;
        }

        public bool IsMetBy(PropertyLookup properties)
        {
            if(!properties.ContainsKey(_sourceTagName))
                return false;
            string tag = properties[_sourceTagName];
            if (tag == null)
                return false;
            return tag == _matches;
        }
    }
}