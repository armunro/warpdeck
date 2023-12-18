// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace COSMIC.Warpdeck.Domain.Property.Rules
{
    public class PropertyRuleModel
    {
        public string UpdateProperty { get; set; }
        public string UpdateValue { get; set; }
        public PropertyRuleConditionModel Criteria { get; set; }

        public override string ToString()
        {
            return $"{Criteria} : {UpdateProperty} = {UpdateValue}";
        }
    }
}