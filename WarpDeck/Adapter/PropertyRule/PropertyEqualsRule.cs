using WarpDeck.Domain.Property;
using WarpDeck.Domain.Property.Rules;

namespace WarpDeck.Adapter.PropertyRule
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
            string tag = properties[_sourceTagName];
            if (tag == null)
                return false;
            return tag == _matches;
        }
    }
}