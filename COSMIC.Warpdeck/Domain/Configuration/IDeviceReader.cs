using COSMIC.Warpdeck.Domain.Device;

namespace COSMIC.Warpdeck.Domain.Configuration
{
    public interface IDeviceReader
    {
        DeviceModelList ReadDevices();
        
    }
}