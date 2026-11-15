using OpenMacroBoard.SDK;

namespace Warpdeck.Core.Domain;

public interface IDeviceService
{
    event Action<IMacroBoard>? DeviceConnected;
    event Action? DeviceDisconnected;
    IMacroBoard? CurrentDevice { get; }
    void Start();
}
