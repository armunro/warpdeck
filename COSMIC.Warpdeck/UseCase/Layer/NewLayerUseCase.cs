using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.Layer;
using COSMIC.Warpdeck.Managers;

namespace COSMIC.Warpdeck.UseCase.Layer
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