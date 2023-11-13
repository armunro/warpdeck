using System.Collections.Generic;
using WarpDeck.Domain.Key.Behavior;

namespace WarpDeck.Domain.Key
{
    public class CreateLayerKeyRequestModel
    {
        public int KeyId { get; set; }
        public BehaviorModel Behavior { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
}