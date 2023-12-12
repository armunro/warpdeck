using COSMIC.Warpdeck.Domain.Device;

namespace COSMIC.Warpdeck.UseCase.DeviceLayer
{
    public class RedrawDeviceLayersUseCase
    {
        private readonly DeviceManager _deviceManager;
        
        public RedrawDeviceLayersUseCase(DeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        public void Invoke(string deviceId)
        {
            _deviceManager.ClearDevice(deviceId);
            _deviceManager.RedrawDevice(deviceId);
           
        }
    }
}