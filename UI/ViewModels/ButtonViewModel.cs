using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Media;
using OpenMacroBoard.SDK;
using Warpdeck.Core.Domain;
using Warpdeck.Infrastructure.Adapters.Yaml;
using Warpdeck.UI.Common;

namespace Warpdeck.UI.ViewModels;

public class ButtonViewModel : ViewModelBase
{
    private int _index;
    private string? _caption;
    private string? _icon;
    private string? _theme;
    private string? _color;
    private string? _fontColor;
    private string? _font;
    private string _actionType = "macro";
    private string _actionValue = string.Empty;
    private bool _isLocked;
    private bool _isBlockedByLockedLayer;
    private string? _blockedMessage;
    private bool _isDropTarget;
    private ImageSource? _previewImage;
    private bool _isSelected;

    public int Index
    {
        get => _index;
        set => SetProperty(ref _index, value);
    }

    public string? Caption
    {
        get => _caption;
        set
        {
            if (SetProperty(ref _caption, value))
            {
                OnPropertyChanged(nameof(DisplayTitle));
            }
        }
    }

    public string? Icon
    {
        get => _icon;
        set
        {
            if (SetProperty(ref _icon, value))
            {
                OnPropertyChanged(nameof(DisplayTitle));
            }
        }
    }

    public string? Theme
    {
        get => _theme;
        set => SetProperty(ref _theme, value);
    }

    public string? Color
    {
        get => _color;
        set => SetProperty(ref _color, value);
    }

    public string? FontColor
    {
        get => _fontColor;
        set => SetProperty(ref _fontColor, value);
    }

    public string? Font
    {
        get => _font;
        set => SetProperty(ref _font, value);
    }

    public string ActionType
    {
        get => _actionType;
        set => SetProperty(ref _actionType, value);
    }

    public string ActionValue
    {
        get => _actionValue;
        set => SetProperty(ref _actionValue, value);
    }

    public bool IsLocked
    {
        get => _isLocked;
        set => SetProperty(ref _isLocked, value);
    }

    public bool IsBlockedByLockedLayer
    {
        get => _isBlockedByLockedLayer;
        set => SetProperty(ref _isBlockedByLockedLayer, value);
    }

    public string? BlockedMessage
    {
        get => _blockedMessage;
        set => SetProperty(ref _blockedMessage, value);
    }

    public bool IsDropTarget
    {
        get => _isDropTarget;
        set => SetProperty(ref _isDropTarget, value);
    }

    public ImageSource? PreviewImage
    {
        get => _previewImage;
        set => SetProperty(ref _previewImage, value);
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public bool HasContent => !string.IsNullOrWhiteSpace(Caption) || !string.IsNullOrWhiteSpace(Icon) || !string.IsNullOrWhiteSpace(ActionValue);

    public string DisplayTitle
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(Caption)) return Caption;
            if (!string.IsNullOrWhiteSpace(Icon)) return System.IO.Path.GetFileNameWithoutExtension(Icon);
            return $"Key {Index}";
        }
    }

    public void CopyFrom(ButtonViewModel source)
    {
        if (source == null) return;
        Caption = source.Caption;
        Icon = source.Icon;
        Theme = source.Theme;
        Color = source.Color;
        FontColor = source.FontColor;
        Font = source.Font;
        ActionType = source.ActionType;
        ActionValue = source.ActionValue;
        IsLocked = source.IsLocked;
    }

    public void SwapWith(ButtonViewModel other)
    {
        if (other == null || other == this) return;

        var tempCaption = Caption;
        var tempIcon = Icon;
        var tempTheme = Theme;
        var tempColor = Color;
        var tempFontColor = FontColor;
        var tempFont = Font;
        var tempActionType = ActionType;
        var tempActionValue = ActionValue;
        var tempIsLocked = IsLocked;

        Caption = other.Caption;
        Icon = other.Icon;
        Theme = other.Theme;
        Color = other.Color;
        FontColor = other.FontColor;
        Font = other.Font;
        ActionType = other.ActionType;
        ActionValue = other.ActionValue;
        IsLocked = other.IsLocked;

        other.Caption = tempCaption;
        other.Icon = tempIcon;
        other.Theme = tempTheme;
        other.Color = tempColor;
        other.FontColor = tempFontColor;
        other.Font = tempFont;
        other.ActionType = tempActionType;
        other.ActionValue = tempActionValue;
        other.IsLocked = tempIsLocked;
    }

    public void Clear()
    {
        Caption = null;
        Icon = null;
        Theme = null;
        Color = null;
        FontColor = null;
        Font = null;
        ActionType = "macro";
        ActionValue = string.Empty;
        IsLocked = false;
    }

    public ButtonConfig? ToConfig()
    {
        if (string.IsNullOrWhiteSpace(Caption) && 
            string.IsNullOrWhiteSpace(Icon) && 
            string.IsNullOrWhiteSpace(Color) && 
            string.IsNullOrWhiteSpace(FontColor) && 
            string.IsNullOrWhiteSpace(Font) && 
            string.IsNullOrWhiteSpace(Theme) && 
            string.IsNullOrWhiteSpace(ActionValue) &&
            !IsLocked)
        {
            return null;
        }

        var config = new ButtonConfig
        {
            Index = Index,
            Caption = string.IsNullOrWhiteSpace(Caption) ? null : Caption,
            Icon = string.IsNullOrWhiteSpace(Icon) ? null : Icon,
            Theme = string.IsNullOrWhiteSpace(Theme) ? null : Theme,
            Color = string.IsNullOrWhiteSpace(Color) ? null : Color,
            FontColor = string.IsNullOrWhiteSpace(FontColor) ? null : FontColor,
            Font = string.IsNullOrWhiteSpace(Font) ? null : Font,
            Locked = IsLocked
        };

        if (!string.IsNullOrWhiteSpace(ActionType) && !string.IsNullOrWhiteSpace(ActionValue))
        {
            config.Action = new ActionConfig
            {
                Type = ActionType,
                Value = ActionValue
            };
        }

        return config;
    }

    public static ButtonViewModel FromConfig(ButtonConfig config)
    {
        return new ButtonViewModel
        {
            Index = config.Index,
            Caption = config.Caption,
            Icon = config.Icon,
            Theme = config.Theme,
            Color = config.Color,
            FontColor = config.FontColor,
            Font = config.Font,
            IsLocked = config.Locked,
            ActionType = config.Action?.Type ?? "macro",
            ActionValue = config.Action?.Value ?? string.Empty
        };
    }

    public void UpdatePreview(IGraphicsAdapter graphicsAdapter, string layerTheme, string layerColor, string? layerFontColor, string? effectiveIconRoot, bool showEmptyButtons = true, string? layerFont = null)
    {
        try
        {
            bool hasCaption = !string.IsNullOrWhiteSpace(Caption);
            bool hasIcon = !string.IsNullOrWhiteSpace(Icon);

            if (!hasCaption && !hasIcon && !showEmptyButtons)
            {
                PreviewImage = WpfImageHelper.ToBitmapSource(KeyBitmap.Black);
                OnPropertyChanged(nameof(HasContent));
                OnPropertyChanged(nameof(DisplayTitle));
                return;
            }

            var btnTheme = !string.IsNullOrWhiteSpace(Theme) ? Theme : layerTheme;
            var hexColor = !string.IsNullOrWhiteSpace(Color) ? Color : layerColor;
            var (r, g, b) = ParseHexColor(hexColor, 0, 0, 0);

            var hexFontColor = !string.IsNullOrWhiteSpace(FontColor) ? FontColor : layerFontColor;
            byte fontR, fontG, fontB;
            if (!string.IsNullOrWhiteSpace(hexFontColor))
            {
                (fontR, fontG, fontB) = ParseHexColor(hexFontColor, 255, 255, 255);
            }
            else
            {
                (fontR, fontG, fontB) = CalculateContrastFontColor(r, g, b);
            }

            var iconPath = Icon;
            var effectiveFont = !string.IsNullOrWhiteSpace(Font) ? Font : layerFont;

            var keyBitmap = graphicsAdapter.DrawKey(r, g, b, fontR, fontG, fontB, Caption, iconPath, btnTheme, false, effectiveFont);
            PreviewImage = WpfImageHelper.ToBitmapSource(keyBitmap);
            OnPropertyChanged(nameof(HasContent));
            OnPropertyChanged(nameof(DisplayTitle));
        }
        catch
        {
            // Ignore preview render failures
        }
    }

    private static (byte R, byte G, byte B) ParseHexColor(string? hex, byte defaultR, byte defaultG, byte defaultB)
    {
        if (string.IsNullOrWhiteSpace(hex)) return (defaultR, defaultG, defaultB);
        hex = hex.Trim().TrimStart('#');
        if (hex.Length == 6 &&
            byte.TryParse(hex.Substring(0, 2), NumberStyles.HexNumber, null, out var r) &&
            byte.TryParse(hex.Substring(2, 2), NumberStyles.HexNumber, null, out var g) &&
            byte.TryParse(hex.Substring(4, 2), NumberStyles.HexNumber, null, out var b))
        {
            return (r, g, b);
        }
        return (defaultR, defaultG, defaultB);
    }

    private static (byte R, byte G, byte B) CalculateContrastFontColor(byte r, byte g, byte b)
    {
        double luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255.0;
        return luminance > 0.5 ? ((byte)0, (byte)0, (byte)0) : ((byte)255, (byte)255, (byte)255);
    }
}
