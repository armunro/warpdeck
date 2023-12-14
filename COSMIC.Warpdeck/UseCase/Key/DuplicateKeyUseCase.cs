using System.Text.Json;
using COSMIC.Warpdeck.Domain.Button;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.Layer;
using COSMIC.Warpdeck.Managers;

namespace COSMIC.Warpdeck.UseCase.Key
{
    public class DuplicateKeyUseCase
    {
        private readonly DeviceManager _deviceManager;

        public DuplicateKeyUseCase(DeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        public void Invoke(string deviceId, string layerId, int keyId, int newKeyId)
        {
            LayerModel layer = _deviceManager.GetDevice(deviceId).Layers[layerId];

            var clone = JsonSerializer.Deserialize<ButtonModel>(JsonSerializer.Serialize(layer.Buttons[keyId]));
            layer.Buttons.Add(newKeyId, clone);
        }
    }
}