using COSMIC.Warpdeck.Domain.Button;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.Layer;
using COSMIC.Warpdeck.Managers;
using COSMIC.Warpdeck.UseCase.DeviceLayer;

namespace COSMIC.Warpdeck.UseCase.Key
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

        public void Invoke(string deviceId, string layerId, string keyId, string newKeyId)
        {
            LayerModel layer = _deviceManager.GetDevice(deviceId).Layers[layerId];

            ButtonModel movingButton = layer.Buttons[keyId];
            layer.Buttons.Remove(keyId);
            if (layer.Buttons.ContainsKey(newKeyId)) //Swap buttons if destination has button already
            {
                ButtonModel oldButton = layer.Buttons[newKeyId];
                layer.Buttons.Remove(newKeyId);
                layer.Buttons.Add(keyId, oldButton);
            }
            _redrawDeviceLayersUseCase.Invoke(deviceId);

            layer.Buttons.Add(newKeyId, movingButton);
        }
    }
}