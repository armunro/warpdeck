using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Managers;

namespace COSMIC.Warpdeck.UseCase.Device
{
    public class UpdateDeviceUseCase
    {
        private readonly DeviceManager _deviceManager;

        public UpdateDeviceUseCase(DeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        public void Invoke(string deviceId, DeviceModel updatedDevice)
        {
            DeviceModel device = _deviceManager.GetDevice(deviceId);
            device.DeviceId = updatedDevice.DeviceId;

            //TODO: Should be changed to updatedevicemodel
            _deviceManager.UnbindDevice(deviceId);
            _deviceManager.BindVirtualDevice(device);

        }   
    }
}