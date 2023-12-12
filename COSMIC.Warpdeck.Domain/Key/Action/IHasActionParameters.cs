using COSMIC.Warpdeck.Domain.Key.Action.Descriptors;

namespace COSMIC.Warpdeck.Domain.Key.Action
{
    public interface IHasActionParameters
    {
        ActionParamDescriptorSet SpecifyParameters();
    }
}