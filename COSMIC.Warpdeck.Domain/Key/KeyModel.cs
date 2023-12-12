using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using COSMIC.Warpdeck.Domain.Key.Behavior;
using COSMIC.Warpdeck.Domain.Property;

namespace COSMIC.Warpdeck.Domain.Key
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