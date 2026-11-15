using OpenMacroBoard.SDK;
using StreamDeckSharp;
using Warpdeck.Core.Domain;

namespace Warpdeck.Infrastructure.Adapters.Windows;

public class StreamDeckService : IDeviceService, IDisposable
{
    private IMacroBoard? _currentDevice;
    private readonly System.Threading.Timer _timer;
    private bool _isScanning;

    public event Action<IMacroBoard>? DeviceConnected;
    public event Action? DeviceDisconnected;

    public IMacroBoard? CurrentDevice => _currentDevice;

    public StreamDeckService()
    {
        _timer = new System.Threading.Timer(ScanForDevice, null, Timeout.Infinite, 2000);
    }

    public void Start()
    {
        _timer.Change(0, 2000);
    }

    private void ScanForDevice(object? state)
    {
        if (_isScanning) return;
        _isScanning = true;

        try
        {
            if (_currentDevice == null)
            {
                try
                {
                    var device = StreamDeck.OpenDevice();
                    if (device != null)
                    {
                        _currentDevice = device;
                        // Use a simple check if ConnectionEventArgs isn't what I thought
                        _currentDevice.ConnectionStateChanged += OnConnectionStateChanged;
                        Console.WriteLine("Stream Deck connected.");
                        DeviceConnected?.Invoke(_currentDevice);
                    }
                }
                catch (Exception ex)
                {
                    // If it's not found, it might throw a specific exception or a general one.
                    // We'll log only if it's NOT a "not found" type error if we can identify it.
                    if (ex.Message.Contains("found", StringComparison.OrdinalIgnoreCase) || 
                        ex.GetType().Name.Contains("NotFound", StringComparison.OrdinalIgnoreCase))
                    {
                         // Silent or debug log
                    }
                    else
                    {
                        Console.WriteLine($"[DEBUG] Info: {ex.Message}");
                    }
                }
            }
        }
        finally
        {
            _isScanning = false;
        }
    }

    private void OnConnectionStateChanged(object? sender, EventArgs e)
    {
        // Many SDKs use a boolean or a state enum. 
        // If we can't be sure of the property name, we can just re-check if the device is still functional
        // or wait for the next scan to confirm if it's gone.
        // For now, let's try to see if it has a common property name.
        
        bool connected = true;
        try 
        {
            // Reflection-like check or just rely on the fact that if this event fires with disconnect,
            // we'll find out. Some SDKs pass ConnectionEventArgs : EventArgs
            var prop = e.GetType().GetProperty("IsConnected");
            if (prop != null)
            {
                connected = (bool)prop.GetValue(e)!;
            }
        }
        catch { }

        if (!connected)
        {
            HandleDisconnect();
        }
    }

    private void HandleDisconnect()
    {
        if (_currentDevice != null)
        {
            Console.WriteLine("Stream Deck disconnected. Waiting for reconnect...");
            _currentDevice.ConnectionStateChanged -= OnConnectionStateChanged;
            
            try 
            {
                _currentDevice.Dispose();
            }
            catch 
            {
                // Ignore dispose errors on disconnected device
            }
            
            _currentDevice = null;
            DeviceDisconnected?.Invoke();
        }
    }

    public void Dispose()
    {
        _timer.Dispose();
        HandleDisconnect();
    }
}
