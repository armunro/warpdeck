

namespace COSMIC.Warpdeck.Domain.Property.Descriptors
{
    public class PropertyDescriptor
    {
        public string Key { get; set; }
        public PropertyType Type { get; set; }
        public string FriendlyName { get; set; }
        public string Description { get; set; }
        public string Default { get; set; }

        public static PropertyDescriptor New()
        {
            return new PropertyDescriptor();
        }
        
        public static PropertyDescriptor Color(string key) => New().Keyed(key).Typed(PropertyType.Color);

        public static PropertyDescriptor Text(string key) => New().Keyed(key).Typed(PropertyType.Text);

        public static PropertyDescriptor Path(string key) => New().Keyed(key).Typed(PropertyType.Path);

        public PropertyDescriptor Keyed(string key)
        {
            Key = key;
            return this;
        }

        public PropertyDescriptor Typed(PropertyType type)
        {
            Type = type;
            return this;
        }

        public PropertyDescriptor Described(string description)
        {
            Description = description;
            return this;
        }

        public PropertyDescriptor Named(string friendlyName)
        {
            FriendlyName = friendlyName;
            return this;
        }

        public PropertyDescriptor WithDefault(string defaultValue)
        {
            Default = defaultValue;
            return this;
        }


        
    }
}