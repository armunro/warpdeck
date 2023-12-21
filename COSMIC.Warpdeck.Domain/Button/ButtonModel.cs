using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using COSMIC.Warpdeck.Domain.Property;
using YamlDotNet.Serialization;

namespace COSMIC.Warpdeck.Domain.Button
{
    [SuppressMessage("ReSharper", "UnusedMember.Global"), SuppressMessage("ReSharper", "UnusedType.Global"),
     SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class ButtonModel
    {
        
        public Dictionary<string, Action.ActionModel> Actions { get; set; } = new();
        [JsonIgnore, YamlIgnore] public ButtonHistoryModel History { get; set; } = new();
        public PropertyLookup Properties { get; set; } = new();
    }
}