using System.Collections.Generic;

namespace WarpDeck.Domain.Property.Descriptors
{
    public class PropertyDescriptorSet
    {
        public Dictionary<string, PropertyDescriptor> Properties { get; set; } = new();
        public string FriendlyName { get; set; }
        public static PropertyDescriptorSet New()
        {
            return new PropertyDescriptorSet();
        }

        public PropertyDescriptorSet Named(string friendlyName)
        {
            FriendlyName = friendlyName;
            return this;
        }

        public PropertyDescriptorSet Has(params PropertyDescriptor[] properties)
        {
            foreach (PropertyDescriptor property in properties)
            {
                Properties.Add(property.Key, property);
            }

            return this;
        }
    
    }
}