using COSMIC.Warpdeck.Domain.Device;

namespace COSMIC.Warpdeck.Domain.DeviceHost;

public interface IDeviceHostClient
{
    DeviceHostHandle OpenDeviceHost(DeviceModel model);
}