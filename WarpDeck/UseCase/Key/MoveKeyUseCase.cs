using WarpDeck.Domain.Device;
using WarpDeck.Domain.Key;
using WarpDeck.Domain.Layer;
using WarpDeck.UseCase.Device;
using WarpDeck.UseCase.DeviceLayer;

namespace WarpDeck.UseCase.Key
{
    public class MoveKeyUseCase
    {
        private readonly DeviceManager _deviceManager;
        private readonly RedrawDeviceLayersUseCase _redrawDeviceLayersUseCase;

        public MoveKeyUseCase(DeviceManager deviceManager, RedrawDeviceLayersUseCase redrawDeviceLayersUseCase)
        {
            _deviceManager = deviceManager;
            _redrawDeviceLayersUseCase = redrawDeviceLayersUseCase;
        }

        public void Invoke(string deviceId, string layerId, int keyId, int newKeyId)
        {
            LayerModel layer = _deviceManager.GetDevice(deviceId).Layers[layerId];

            KeyModel movingKey = layer.Keys[keyId];
            layer.Keys.Remove(keyId);
            if (layer.Keys.ContainsKey(newKeyId)) //Swap keys if destination has key already
            {
                KeyModel oldKey = layer.Keys[newKeyId];
                layer.Keys.Remove(newKeyId);
                layer.Keys.Add(keyId, oldKey);
            }
            _redrawDeviceLayersUseCase.Invoke(deviceId);

            layer.Keys.Add(newKeyId, movingKey);
        }
    }
}