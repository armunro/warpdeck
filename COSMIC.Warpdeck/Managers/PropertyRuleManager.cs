using System.Linq;
using Autofac;
using COSMIC.Warpdeck.Domain.Key;
using COSMIC.Warpdeck.Domain.Property.Descriptors;
using COSMIC.Warpdeck.Domain.Property.Rules;

namespace COSMIC.Warpdeck.Managers
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
                    //TODO: Cannot move this to Domain package because WarpdeckApp will not be available
                    WarpdeckApp.Container.ResolveNamed<IPropertyRule>(ruleModel.Criteria.Type, criteriaParameters);
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