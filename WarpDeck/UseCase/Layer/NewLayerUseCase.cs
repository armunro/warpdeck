using WarpDeck.Domain.Device;
using WarpDeck.Domain.Layer;
using WarpDeck.UseCase.Device;

namespace WarpDeck.UseCase.Layer
{
    public class NewLayerUseCase
    {
        private readonly DeviceManager _deviceManager;

        public NewLayerUseCase(DeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        public string Invoke(string deviceId, string layerId)
        {
            _deviceManager.GetDevice(deviceId).Layers.Add(layerId, new LayerModel()
            {
                LayerId = layerId
            });
            return layerId;
        }
    }
}