using WarpDeck.Domain.Key.Action.Descriptors;

namespace WarpDeck.Domain.Key.Action
{
    public interface IHasActionParameters
    {
        ActionParamDescriptorSet SpecifyParameters();
    }
}