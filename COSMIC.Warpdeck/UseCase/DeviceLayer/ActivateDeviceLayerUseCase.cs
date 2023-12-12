using COSMIC.Warpdeck.Domain.Device;

namespace COSMIC.Warpdeck.UseCase.DeviceLayer
{
    public class ActivateDeviceLayerUseCase
    {
        private readonly DeviceManager _deviceManager;

        public ActivateDeviceLayerUseCase(DeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        public void Invoke(string deviceId, string layerId)
        {
            if (!_deviceManager.GetDevice(deviceId).IsLayerActive(layerId))
            {
                _deviceManager.GetDevice(deviceId).ActivateLayer(layerId);
                
            }
            
        }
    }
}