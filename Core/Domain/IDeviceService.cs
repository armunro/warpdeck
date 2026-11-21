using OpenMacroBoard.SDK;

namespace Warpdeck.Core.Domain;

public interface IDeviceService
{
    event Action<IMacroBoard>? DeviceConnected;
    event Action? DeviceDisconnected;
    IMacroBoard? CurrentDevice { get; }
    byte Brightness { get; }
    void SetBrightness(byte percent);
    void Start();
}
