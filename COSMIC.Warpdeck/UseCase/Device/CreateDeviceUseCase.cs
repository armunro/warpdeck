using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Managers;

namespace COSMIC.Warpdeck.UseCase.Device
{
    public class CreateDeviceUseCase
    {
        private readonly DeviceManager _deviceManager;

        public CreateDeviceUseCase(DeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        public void Invoke(string deviceId, DeviceModel newDeviceModel)
        {
            newDeviceModel.DeviceId = deviceId;
            
            _deviceManager.BindVirtualDevice(newDeviceModel);
            
        }
    }
}