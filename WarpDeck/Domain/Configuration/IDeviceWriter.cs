using WarpDeck.Domain.Device;

namespace WarpDeck.Domain.Configuration
{
    public interface IDeviceWriter
    {
        void WriteDeviceModel(DeviceModel device);
    }
}