using COSMIC.Warpdeck.Domain.Property.Descriptors;

namespace COSMIC.Warpdeck.Domain.Property
{
    public interface IHasProperties
    {
        PropertyDescriptorSet SpecifyProperties();
    }
}