using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using WarpDeck.Domain.Key.Behavior;
using WarpDeck.Domain.Property;

namespace WarpDeck.Domain.Key
{
    [SuppressMessage("ReSharper", "UnusedMember.Global"), SuppressMessage("ReSharper", "UnusedType.Global"),
     SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class KeyModel
    {
        public BehaviorModel Behavior { get; set; }
        [JsonIgnore] public KeyHistoryModel History { get; set; } = new();
        public PropertyLookup Properties { get; set; } = new();
    }
}