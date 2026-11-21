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
    private readonly List<Layer> _activeLayers = new();
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
        _deck.SetBrightness(_deviceService.Brightness);
        
        int keySize = 96;
        if (deck.Keys.Count == 32)
        {
            keySize = 96;
        }
        else if (deck.Keys.Count == 15)
        {
            keySize = 72;
        }
        else if (deck.Keys.Count == 6)
        {
            keySize = 80;
        }
        else if (deck.Keys.Count == 8)
        {
            keySize = 96;
        }
        _graphics.SetKeySize(keySize, keySize);
        Console.WriteLine($"[DEVICE] Type: {deck.GetType().FullName}, Keys: {deck.Keys.Count}, KeySize: {keySize}x{keySize}");
        
        // Force re-apply layers if we have active layers
        lock (_layers)
        {
            if (_activeLayers.Count > 0)
            {
                ApplyActiveLayers();
            }
        }
    }

    public void SetBrightness(byte percent)
    {
        _deviceService.SetBrightness(percent);
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
        Layer? effectiveLayer = null;
        lock (_layers)
        {
            effectiveLayer = GetEffectiveLayerForKey(e.Key);
        }

        if (effectiveLayer != null)
        {
            UpdateKeyBitmap(e.Key, e.IsDown);

            if (e.IsDown)
            {
                if (effectiveLayer.Actions.TryGetValue(e.Key, out var action))
                {
                    action.Execute();
                }
            }
        }
    }

    public void AddLayer(Layer layer)
    {
        lock (_layers)
        {
            _layers.Add(layer);
            _layers.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }
    }

    public void SetLayers(IEnumerable<Layer> layers)
    {
        lock (_layers)
        {
            _layers.Clear();
            _layers.AddRange(layers);
            _layers.Sort((a, b) => b.Priority.CompareTo(a.Priority));
            _activeLayers.Clear();
            _activeLayer = null; // Force re-evaluation
        }
    }

    private void CheckConditions(object? state)
    {
        LogForegroundWindow();

        List<Layer> newActiveLayers = new();

        lock (_layers)
        {
            foreach (var layer in _layers)
            {
                if (layer.Condition == null || layer.Condition.IsMet())
                {
                    newActiveLayers.Add(layer);
                }
            }

            bool changed = newActiveLayers.Count != _activeLayers.Count ||
                           !newActiveLayers.SequenceEqual(_activeLayers);

            if (changed)
            {
                _activeLayers.Clear();
                _activeLayers.AddRange(newActiveLayers);
                _activeLayer = _activeLayers.FirstOrDefault();
                ApplyActiveLayers();
            }
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

    private void ApplyActiveLayers()
    {
        if (_deck == null) return;

        if (_activeLayers.Count == 0)
        {
            for (int i = 0; i < _deck.Keys.Count; i++)
                _deck.SetKeyBitmap(i, KeyBitmap.Black);
            return;
        }

        Console.WriteLine($"Activating active layers: {string.Join(" -> ", _activeLayers.Select(l => l.Name))}");

        for (int i = 0; i < _deck.Keys.Count; i++)
        {
            UpdateKeyBitmap(i, false);
        }
    }

    private Layer? GetEffectiveLayerForKey(int keyIndex)
    {
        if (_activeLayers.Count == 0) return null;

        // Check if any active layer has explicitly locked this button slot (e.g. Default layer window actions).
        // Locked buttons cannot be covered or blocked by any other layer.
        foreach (var layer in _activeLayers)
        {
            if (layer.LockedKeys.Contains(keyIndex))
            {
                return layer;
            }
        }

        // Find highest-priority active layer that explicitly defines a button on this slot
        foreach (var layer in _activeLayers)
        {
            if (layer.DefinedKeys.Contains(keyIndex))
            {
                return layer;
            }
        }

        // If no active layer explicitly defined this key, fall back to the base (last/lowest-priority, usually Default) active layer
        return _activeLayers.LastOrDefault() ?? _activeLayers.FirstOrDefault();
    }

    private void UpdateKeyBitmap(int keyIndex, bool isPressed)
    {
        if (_deck == null || _activeLayers.Count == 0) return;

        var effectiveLayer = GetEffectiveLayerForKey(keyIndex);
        if (effectiveLayer == null)
        {
            _deck.SetKeyBitmap(keyIndex, KeyBitmap.Black);
            return;
        }

        bool hasCaption = effectiveLayer.Captions.TryGetValue(keyIndex, out var caption) && !string.IsNullOrWhiteSpace(caption);
        bool hasIcon = effectiveLayer.Icons.TryGetValue(keyIndex, out var icon) && !string.IsNullOrWhiteSpace(icon);
        effectiveLayer.Themes.TryGetValue(keyIndex, out var theme);
        effectiveLayer.Fonts.TryGetValue(keyIndex, out var font);

        if (!hasCaption && !hasIcon && !effectiveLayer.ShowEmptyButtons)
        {
            _deck.SetKeyBitmap(keyIndex, KeyBitmap.Black);
            return;
        }

        KeyBitmap bitmap = KeyBitmap.Black;
        if (effectiveLayer.Keys.TryGetValue(keyIndex, out var keyBmp) && !isPressed)
            bitmap = keyBmp;

        if (hasCaption || hasIcon || !string.IsNullOrWhiteSpace(theme))
        {
            var (r, g, b) = effectiveLayer.Colors.TryGetValue(keyIndex, out var color) ? color : (R: (byte)0, G: (byte)0, B: (byte)0);
            var (fr, fg, fb) = effectiveLayer.FontColors.TryGetValue(keyIndex, out var fColor) ? fColor : (R: (byte)255, G: (byte)255, B: (byte)255);
            bitmap = _graphics.DrawKey(r, g, b, fr, fg, fb, caption, icon, theme, isPressed, font);
        }

        _deck.SetKeyBitmap(keyIndex, bitmap);
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