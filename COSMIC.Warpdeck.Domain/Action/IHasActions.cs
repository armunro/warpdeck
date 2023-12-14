using COSMIC.Warpdeck.Domain.Action.Descriptors;

namespace COSMIC.Warpdeck.Domain.Action
{
    public interface IHasActions
    {
        ActionDescriptorSet SpecifyActions();
    }
}