using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.DeviceHost;

namespace COSMIC.Warpdeck.Windows.Adapter;

public class WindowsFormsDeviceHostClient : IDeviceHostClient
{
    public DeviceHostHandle OpenDeviceHost(DeviceModel model)
    {
        return new WinFormsDeviceHostHandle()
        {
            Device = model,
            WinForm = new DeviceHost(model)
        };
    }
}