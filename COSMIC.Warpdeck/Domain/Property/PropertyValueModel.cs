using System.Diagnostics.CodeAnalysis;

namespace COSMIC.Warpdeck.Domain.Property
{
    [SuppressMessage("ReSharper", "UnusedMember.Global"), 
     SuppressMessage("ReSharper", "UnusedType.Global"),
     SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class PropertyValueModel
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public static PropertyValueModel Create(string name, string value)
        {
            return new PropertyValueModel { Name = name, Value = value };
        }

        public static string ToPropertyString(PropertyValueModel propertyValueModel)
        {
            return propertyValueModel.Value;
        }
    }
}