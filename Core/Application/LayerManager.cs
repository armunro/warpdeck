using System.Text;
using OpenMacroBoard.SDK;
using Warpdeck.Core.Domain;

namespace Warpdeck.Core.Application;

public class LayerManager : IDisposable
{
    private readonly IDeviceService _deviceService;
    private IMacroBoard? _deck;
    private readonly IGraphicsAdapter _graphics;
    private readonly IWindowService _windowService;
    private readonly List<Layer> _layers = new();
    private Layer? _activeLayer;
    private string? _lastProcessName;
    private string? _lastWindowTitle;
    private readonly System.Threading.Timer _timer;

    public LayerManager(IDeviceService deviceService, IGraphicsAdapter graphics, IWindowService windowService)
    {
        _deviceService = deviceService;
        _graphics = graphics;
        _windowService = windowService;

        _deviceService.DeviceConnected += OnDeviceConnected;
        _deviceService.DeviceDisconnected += OnDeviceDisconnected;

        if (_deviceService.CurrentDevice != null)
        {
            OnDeviceConnected(_deviceService.CurrentDevice);
        }

        _timer = new System.Threading.Timer(CheckConditions, null, 0, 1000);
    }

    private void OnDeviceConnected(IMacroBoard deck)
    {
        _deck = deck;
        _deck.KeyStateChanged += OnKeyStateChanged;
        _deck.SetBrightness(100);
        
        // Force re-apply layer if we have one
        if (_activeLayer != null)
        {
            ApplyLayer(_activeLayer);
        }
    }

    private void OnDeviceDisconnected()
    {
        if (_deck != null)
        {
            _deck.KeyStateChanged -= OnKeyStateChanged;
            _deck = null;
        }
    }

    private void OnKeyStateChanged(object? sender, KeyEventArgs e)
    {
        if (e.IsDown && _activeLayer != null)
        {
            if (_activeLayer.Actions.TryGetValue(e.Key, out var action))
            {
                action.Execute();
            }
        }
    }

    public void AddLayer(Layer layer)
    {
        _layers.Add(layer);
        _layers.Sort((a, b) => b.Priority.CompareTo(a.Priority));
    }

    public void SetLayers(IEnumerable<Layer> layers)
    {
        lock (_layers)
        {
            _layers.Clear();
            _layers.AddRange(layers);
            _layers.Sort((a, b) => b.Priority.CompareTo(a.Priority));
            _activeLayer = null; // Force re-evaluation
        }
    }

    private void CheckConditions(object? state)
    {
        LogForegroundWindow();

        Layer? bestLayer = null;

        lock (_layers)
        {
            foreach (var layer in _layers)
            {
                if (layer.Condition == null || layer.Condition.IsMet())
                {
                    bestLayer = layer;
                    break;
                }
            }
        }

        if (bestLayer != _activeLayer)
        {
            _activeLayer = bestLayer;
            ApplyLayer(_activeLayer);
        }
    }

    private void LogForegroundWindow()
    {
        var info = _windowService.GetForegroundWindowInfo();
        if (info == null) return;

        if (info.ProcessName != _lastProcessName || info.Title != _lastWindowTitle)
        {
            Console.WriteLine($"[DEBUG] Window Changed: {info.Title} ({info.ProcessName})");
            _lastProcessName = info.ProcessName;
            _lastWindowTitle = info.Title;
        }
    }

    private void ApplyLayer(Layer? layer)
    {
        if (_deck == null) return;

        if (layer == null)
        {
            for (int i = 0; i < _deck.Keys.Count; i++)
                _deck.SetKeyBitmap(i, KeyBitmap.Black);
            return;
        }

        Console.WriteLine($"Activating layer: {layer.Name}");

        for (int i = 0; i < _deck.Keys.Count; i++)
        {
            KeyBitmap bitmap = KeyBitmap.Black;
            if (layer.Keys.TryGetValue(i, out var keyBmp))
                bitmap = keyBmp;

            if (layer.Captions.TryGetValue(i, out var caption))
            {
                var (r, g, b) = layer.Colors.TryGetValue(i, out var color) ? color : (R: (byte)0, G: (byte)0, B: (byte)0);
                var (fr, fg, fb) = layer.FontColors.TryGetValue(i, out var fColor) ? fColor : (R: (byte)255, G: (byte)255, B: (byte)255);
                bitmap = _graphics.DrawCaption(r, g, b, fr, fg, fb, caption);
            }

            _deck.SetKeyBitmap(i, bitmap);
        }
    }

    public void Dispose()
    {
        _deviceService.DeviceConnected -= OnDeviceConnected;
        _deviceService.DeviceDisconnected -= OnDeviceDisconnected;
        if (_deck != null)
        {
            _deck.KeyStateChanged -= OnKeyStateChanged;
        }
        _timer.Dispose();
    }
}