using WarpDeck.Domain.Property.Descriptors;

namespace WarpDeck.Presentation.Controllers.Models
{
    public class TypePropertiesResponseModel
    {
        public string TypeName { get; set; }
        public PropertyDescriptorSet Properties { get; set; }
    }
}