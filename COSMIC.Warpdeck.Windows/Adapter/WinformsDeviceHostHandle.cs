using System.Windows.Forms;
using COSMIC.Warpdeck.Domain.DeviceHost;

namespace COSMIC.Warpdeck.Windows.Adapter;

public class WinFormsDeviceHostHandle: DeviceHostHandle
{
    public Form WinForm { get; set; }
}