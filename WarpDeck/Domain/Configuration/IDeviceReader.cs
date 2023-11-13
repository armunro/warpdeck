using WarpDeck.Domain.Device;

namespace WarpDeck.Domain.Configuration
{
    public interface IDeviceReader
    {
        DeviceModelList ReadDevices();
        
    }
}