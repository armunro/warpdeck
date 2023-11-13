// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace WarpDeck.Domain.Property.Rules
{
    public class PropertyRuleModel
    {
        public string TargetTagName { get; set; }
        public string TargetTagValue { get; set; }
        public PropertyRuleConditionModel Criteria { get; set; }

        public override string ToString()
        {
            return $"{Criteria} : {TargetTagName} = {TargetTagValue}";
        }
    }
}