using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Managers;

namespace COSMIC.Warpdeck.UseCase.DeviceLayer
{
    public class DeactivateDeviceLayerUseCase
    {
        private readonly DeviceManager _deviceManager;

        public DeactivateDeviceLayerUseCase(DeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        public void Invoke(string deviceId, string layerId)
        {
            if (_deviceManager.GetDevice(deviceId).IsLayerActive(layerId))
            {
                _deviceManager.GetDevice(deviceId).DeactivateLayer(layerId);
            }
        }
    }
}