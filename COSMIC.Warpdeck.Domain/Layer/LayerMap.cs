namespace COSMIC.Warpdeck.Domain.Layer
{
    public class LayerMap : Dictionary<string, LayerModel>
    {
        public LayerModel GetLayerById(string layerId)
        {
            TryGetValue(layerId, out var returnLayerOrNull);
            return returnLayerOrNull;
        }
    }
}