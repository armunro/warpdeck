using COSMIC.Warpdeck.Domain.Property.Descriptors;

namespace COSMIC.Warpdeck.Web.Controllers.Models
{
    public class TypePropertiesResponseModel
    {
        public string TypeName { get; set; }
        public PropertyDescriptorSet Properties { get; set; }
    }
}