using WarpDeck.Domain.Device;

namespace WarpDeck.UseCase.Device
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
            
            _deviceManager.BindDevice(newDeviceModel);
            
        }
    }
}