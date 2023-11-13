using System.Text.Json;
using WarpDeck.Domain.Device;
using WarpDeck.Domain.Key;
using WarpDeck.Domain.Layer;
using WarpDeck.UseCase.Device;

namespace WarpDeck.UseCase.Key
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

            var clone = JsonSerializer.Deserialize<KeyModel>(JsonSerializer.Serialize(layer.Keys[keyId]));
            layer.Keys.Add(newKeyId, clone);
        }
    }
}