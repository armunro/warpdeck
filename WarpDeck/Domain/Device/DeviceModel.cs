using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using WarpDeck.Domain.Key;
using WarpDeck.Domain.Layer;
using WarpDeck.Domain.Layer.Exceptions;
using WarpDeck.Domain.Monitor.Rules;
using WarpDeck.Domain.Property.Rules;

namespace WarpDeck.Domain.Device
{
    [SuppressMessage("ReSharper", "UnusedMember.Global"), SuppressMessage("ReSharper", "UnusedType.Global"),
     SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class DeviceModel
    {
        public DeviceInfo Info { get; set; } = new();
        public string DeviceId { get; set; }
        [JsonIgnore] public LayerMap Layers { get; set; } = new();
        [JsonIgnore] public MonitorRuleList MonitorRules { get; set; }
        [JsonIgnore] public LayerMap ActiveLayers { get; set; } = new();
        [JsonIgnore] public KeyMap KeyStates { get; set; } = new();
        [JsonIgnore] public PropertyRuleModelList PropertyRules { get; set; } = new();
        

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

            foreach (int key in Layers[layerId].Keys.Keys)
            {
                KeyStates[key] = null;
            }
        }
    }
}