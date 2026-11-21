using System.IO;
using OpenMacroBoard.SDK;
using Warpdeck.Core.Domain;
using Warpdeck.Infrastructure.Adapters.Windows;

namespace Warpdeck.Infrastructure.Adapters.Yaml;

public class YamlLayerProvider : ILayerProvider, IDisposable
{
    private readonly string _configPath;
    private int _keyCount;
    private readonly IWindowService _windowService;
    private readonly IGraphicsAdapter? _graphicsAdapter;
    private readonly IDeviceService? _deviceService;
    private readonly FileSystemWatcher _watcher;
    private List<Layer> _currentLayers = new();

    public event Action? LayersChanged;

    public YamlLayerProvider(string configPath, IWindowService windowService, IGraphicsAdapter? graphicsAdapter = null, IDeviceService? deviceService = null)
    {
        _configPath = Path.GetFullPath(configPath);
        _windowService = windowService;
        _graphicsAdapter = graphicsAdapter;
        _deviceService = deviceService;

        // Default key count for common Stream Decks if not yet connected
        // We will update it when we reload if we have a device, 
        // but for now let's just use a reasonable default or 0
        _keyCount = 32; 

        var directory = Path.GetDirectoryName(_configPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        var fileName = Path.GetFileName(_configPath);

        _watcher = new FileSystemWatcher(string.IsNullOrEmpty(directory) ? "." : directory)
        {
            Filter = fileName,
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size
        };

        _watcher.Changed += OnFileChanged;
        _watcher.Created += OnFileChanged;
        _watcher.Renamed += OnFileChanged;
        _watcher.EnableRaisingEvents = true;

        Reload();
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        // Add a small delay to avoid trying to read while the file is being written
        Thread.Sleep(100);
        Reload();
    }

    private void Reload()
    {
        try
        {
            var newLayers = LoadLayers();
            _currentLayers = newLayers.ToList();
            Console.WriteLine("Configuration reloaded successfully.");
            LayersChanged?.Invoke();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to load configuration: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"  Inner: {ex.InnerException.Message}");
            }
            Console.WriteLine("Please fix the error and save the file to retry.");
        }
    }

    public IEnumerable<Layer> GetLayers() => _currentLayers;

    private IEnumerable<Layer> LoadLayers()
    {
        var config = ConfigLoader.Load(_configPath);
        if (config.Brightness.HasValue && _deviceService != null)
        {
            _deviceService.SetBrightness(config.Brightness.Value);
        }

        if (_graphicsAdapter != null)
        {
            var iconRootPath = config.IconRoot;
            if (!string.IsNullOrWhiteSpace(iconRootPath) && !Path.IsPathRooted(iconRootPath))
            {
                var configDir = Path.GetDirectoryName(_configPath);
                if (!string.IsNullOrEmpty(configDir))
                {
                    iconRootPath = Path.GetFullPath(Path.Combine(configDir, iconRootPath));
                }
            }
            _graphicsAdapter.SetIconRoot(iconRootPath);
        }

        var layers = new List<Layer>();

        foreach (var layerConfig in config.Layers)
        {
            ICondition? condition = null;
            if (layerConfig.Condition != null)
            {
                condition = new WindowCondition(
                    _windowService,
                    layerConfig.Condition.ProcessName,
                    layerConfig.Condition.WindowTitle
                );
            }

            var layer = new Layer(layerConfig.Name, condition, layerConfig.Priority, layerConfig.ShowEmptyButtons);
            
            // Parse colors
            var (baseR, baseG, baseB) = ParseColorBytes(layerConfig.Color);
            var (baseFontR, baseFontG, baseFontB) = layerConfig.FontColor != null 
                ? ParseColorBytes(layerConfig.FontColor) 
                : GetContrastColor(baseR, baseG, baseB);
            
            var baseKeyBitmap = ParseColor(baseR, baseG, baseB);

            for (int i = 0; i < _keyCount; i++)
            {
                layer.Keys[i] = baseKeyBitmap;
                layer.Colors[i] = (baseR, baseG, baseB);
                layer.FontColors[i] = (baseFontR, baseFontG, baseFontB);
                if (layerConfig.Theme != null)
                {
                    layer.Themes[i] = layerConfig.Theme;
                }
                if (layerConfig.Font != null)
                {
                    layer.Fonts[i] = layerConfig.Font;
                }
            }

            // Apply specific button overrides
            foreach (var buttonConfig in layerConfig.Buttons)
            {
                layer.DefinedKeys.Add(buttonConfig.Index);
                if (buttonConfig.Locked)
                {
                    layer.LockedKeys.Add(buttonConfig.Index);
                }
                var (btnR, btnG, btnB) = layer.Colors[buttonConfig.Index];
                
                if (buttonConfig.Color != null)
                {
                    (btnR, btnG, btnB) = ParseColorBytes(buttonConfig.Color);
                    layer.Keys[buttonConfig.Index] = ParseColor(btnR, btnG, btnB);
                    layer.Colors[buttonConfig.Index] = (btnR, btnG, btnB);
                    
                    // If background color changes but font color isn't explicitly set for the button,
                    // we should re-evaluate the contrast color based on the new background.
                    if (buttonConfig.FontColor == null)
                    {
                        layer.FontColors[buttonConfig.Index] = GetContrastColor(btnR, btnG, btnB);
                    }
                }

                if (buttonConfig.FontColor != null)
                {
                    layer.FontColors[buttonConfig.Index] = ParseColorBytes(buttonConfig.FontColor);
                }

                if (buttonConfig.Caption != null)
                {
                    layer.Captions[buttonConfig.Index] = buttonConfig.Caption;
                }

                if (buttonConfig.Icon != null)
                {
                    var iconPath = buttonConfig.Icon;
                    var localIconRoot = buttonConfig.IconRoot ?? layerConfig.IconRoot;
                    if (!string.IsNullOrWhiteSpace(localIconRoot) && !Path.IsPathRooted(iconPath))
                    {
                        var resolvedRoot = localIconRoot;
                        if (!Path.IsPathRooted(resolvedRoot))
                        {
                            var configDir = Path.GetDirectoryName(_configPath);
                            if (!string.IsNullOrEmpty(configDir))
                            {
                                resolvedRoot = Path.GetFullPath(Path.Combine(configDir, resolvedRoot));
                            }
                        }
                        iconPath = Path.Combine(resolvedRoot, iconPath);
                    }
                    layer.Icons[buttonConfig.Index] = iconPath;
                }

                if (buttonConfig.Theme != null)
                {
                    layer.Themes[buttonConfig.Index] = buttonConfig.Theme;
                }

                if (buttonConfig.Font != null)
                {
                    layer.Fonts[buttonConfig.Index] = buttonConfig.Font;
                }

                if (buttonConfig.Action != null)
                {
                    if (buttonConfig.Action.Type.Equals("macro", StringComparison.OrdinalIgnoreCase))
                    {
                        layer.Actions[buttonConfig.Index] = new MacroAction(buttonConfig.Action.Value);
                    }
                    else if (buttonConfig.Action.Type.Equals("brightness", StringComparison.OrdinalIgnoreCase) && _deviceService != null)
                    {
                        layer.Actions[buttonConfig.Index] = new BrightnessAction(_deviceService, buttonConfig.Action.Value);
                    }
                }
            }

            layers.Add(layer);
        }

        return layers;
    }

    private (byte R, byte G, byte B) ParseColorBytes(string colorHex)
    {
        try
        {
            var hex = colorHex.TrimStart('#');
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return (r, g, b);
        }
        catch
        {
            return (0, 0, 0);
        }
    }

    private static KeyBitmap ParseColor(byte r, byte g, byte b)
    {
        return KeyBitmap.Create.FromRgb(r, g, b);
    }

    private (byte R, byte G, byte B) GetContrastColor(byte r, byte g, byte b)
    {
        // Standard luminance calculation
        // https://www.w3.org/TR/WCAG20/#relativeluminancedef
        double luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255;
        return luminance > 0.5 ? ((byte)0, (byte)0, (byte)0) : ((byte)255, (byte)255, (byte)255);
    }

    public void Dispose()
    {
        _watcher.Dispose();
    }
}
