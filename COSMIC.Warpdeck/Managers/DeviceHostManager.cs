using System.Collections.Generic;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.DeviceHost;

namespace COSMIC.Warpdeck.Managers;

public class DeviceHostManager
{
    private  IDeviceHostClient _hostClient;
    private readonly DeviceManager _deviceManager;
    public Dictionary<string, DeviceHostHandle> _handles;

    public DeviceHostManager(DeviceManager deviceManager)
    {
        
        _deviceManager = deviceManager;
        _handles = new Dictionary<string, DeviceHostHandle>();
    }

    public void RegisterHostClient(IDeviceHostClient client)
    {
        _hostClient = client;
    }

    public void StartDeviceHost(string deviceId)
    {
        DeviceModel deviceModel = _deviceManager.GetDevice(deviceId);
        DeviceHostHandle deviceHostHandle = _hostClient.OpenDeviceHost(deviceModel);
        _handles.Add(deviceId, deviceHostHandle);
    }
}