using System.Linq;
using Autofac;
using WarpDeck.Domain.Key;
using WarpDeck.Domain.Property.Descriptors;

namespace WarpDeck.Domain.Property.Rules
{
    public class PropertyRuleManager
    {
        public PropertyRuleModelList Rules { get; } = new();

  
        
        public string GetProperty( KeyModel key, PropertyDescriptor property, bool noDefault = false)
        {
            //check for explicit tags
            if (key.Properties.HasProperty(property.Key))
                return key.Properties.GetProperty(property.Key);

            //check for style rules
            var applicableRules = Rules.Where(x => x.TargetTagName == property.Key);
            if (!applicableRules.Any())
                return property.Default;

            foreach (PropertyRuleModel ruleModel in applicableRules)
            {
                var criteriaParameters = ruleModel.Criteria.Parameters.Select(x => new NamedParameter(x.Key, x.Value))
                    .ToArray();
                IPropertyRule propertyRule =
                    WarpDeckApp.Container.ResolveNamed<IPropertyRule>(ruleModel.Criteria.Type, criteriaParameters);
                if (propertyRule.IsMetBy(key.Properties))
                    return ruleModel.TargetTagValue;
            }

            if (noDefault)
                return "";
            else
                return property.Default;
        }
    }
}