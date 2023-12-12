using COSMIC.Warpdeck.Domain.Device;

namespace COSMIC.Warpdeck.Domain.Configuration
{
    public interface IDeviceWriter
    {
        void WriteDeviceModel(DeviceModel device);
    }
}