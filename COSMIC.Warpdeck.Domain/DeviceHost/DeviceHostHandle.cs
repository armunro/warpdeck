using COSMIC.Warpdeck.Domain.Device;

namespace COSMIC.Warpdeck.Domain.DeviceHost;

public abstract class DeviceHostHandle
{
    public DeviceModel Device { get; set; }
}