using COSMIC.Warpdeck.Domain.Key.Behavior;

namespace COSMIC.Warpdeck.Domain.Key
{
    public class CreateLayerKeyRequestModel
    {
        public int KeyId { get; set; }
        public BehaviorModel Behavior { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
}