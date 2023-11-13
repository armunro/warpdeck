using WarpDeck.Domain.Property.Descriptors;

namespace WarpDeck.Domain.Property
{
    public interface IHasProperties
    {
        PropertyDescriptorSet SpecifyProperties();
    }
}