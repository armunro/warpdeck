using Warpdeck.Core.Domain;

namespace Warpdeck.Core.Domain;

public interface ILayerProvider
{
    IEnumerable<Layer> GetLayers();
    event Action? LayersChanged;
}
