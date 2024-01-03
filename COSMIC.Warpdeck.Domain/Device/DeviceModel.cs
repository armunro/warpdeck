using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using COSMIC.Warpdeck.Domain.Action;
using COSMIC.Warpdeck.Domain.Button;
using COSMIC.Warpdeck.Domain.Layer;
using COSMIC.Warpdeck.Domain.Monitor.Rules;
using COSMIC.Warpdeck.Domain.Property.Rules;
using YamlDotNet.Serialization;

namespace COSMIC.Warpdeck.Domain.Device
{
    [SuppressMessage("ReSharper", "UnusedMember.Global"), SuppressMessage("ReSharper", "UnusedType.Global"),
     SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class DeviceModel
    {
        public DeviceInfo Info { get; set; } = new();
        public string DeviceId { get; set; }
        [JsonIgnore, YamlIgnore] public LayerMap Layers { get; set; } = new();
        [JsonIgnore, YamlIgnore] public LayerMap ActiveLayers { get; set; } = new();
        [JsonIgnore, YamlIgnore] public MonitorRuleList MonitorRules { get; set; } = new();
        [JsonIgnore, YamlIgnore] public ButtonMap ButtonStates { get; set; } = new();
        [JsonIgnore, YamlIgnore] public List<PropertyRuleModel> PropertyRules { get; set; } = new();
        [JsonIgnore, YamlIgnore] public Dictionary<String,ActionModel> ActionsCombined { get; set; } = new();
        
        public bool IsLayerActive(string layerId) => ActiveLayers.ContainsKey(layerId);

        public void ActivateLayer(string layerId)
        {
            if (!Layers.ContainsKey(layerId))
                throw new LayerDoesNotExistException(DeviceId, layerId);
            if (!ActiveLayers.ContainsKey(layerId))
                ActiveLayers.Add(layerId, Layers[layerId]);
        }

        public void DeactivateLayer(string layerId)
        {
            if (ActiveLayers.ContainsKey(layerId))
            {
                ActiveLayers.Remove(layerId);
            }

            foreach (string key in Layers[layerId].Buttons.Keys)
            {
                ButtonStates[key] = null;
            }
        }
    }
}