using System.Collections.Generic;
using System.Linq;
using Autofac;
using COSMIC.Warpdeck.Domain.Button;
using COSMIC.Warpdeck.Domain.Property.Descriptors;
using COSMIC.Warpdeck.Domain.Property.Rules;

namespace COSMIC.Warpdeck.Managers
{
    public class PropertyRuleManager
    {
        public List<PropertyRuleModel> Rules { get; } = new();

  
        
        public string GetProperty( ButtonModel button, PropertyDescriptor property, bool noDefault = false)
        {
            //check for explicit tags
            if (button.Properties.HasProperty(property.Key))
                return button.Properties.GetProperty(property.Key);

            //check for style rules
            var applicableRules = Rules.Where(x => x.UpdateProperty == property.Key);
            if (!applicableRules.Any())
                return property.Default;

            foreach (PropertyRuleModel ruleModel in applicableRules)
            {
                var criteriaParameters = ruleModel.Criteria.Parameters.Select(x => new NamedParameter(x.Key, x.Value))
                    .ToArray();
                IPropertyRule propertyRule =
                    //TODO: Cannot move this to Domain package because WarpdeckAppContext will not be available
                    WarpdeckAppContext.Container.ResolveNamed<IPropertyRule>(ruleModel.Criteria.Type, criteriaParameters);
                if (propertyRule.IsMetBy(button.Properties))
                    return ruleModel.UpdateValue;
            }

            if (noDefault)
                return "";
            else
                return property.Default;
        }
    }
}