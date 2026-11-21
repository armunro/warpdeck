using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Warpdeck.Core.Domain;
using Warpdeck.Infrastructure.Adapters.Yaml;
using Warpdeck.UI.Common;

namespace Warpdeck.UI.ViewModels;

public class LayerViewModel : ViewModelBase
{
    private string _name = "New Layer";
    private int _priority;
    private string _theme = "cyber";
    private string _color = "#000000";
    private string? _fontColor;
    private string? _font;
    private string? _iconRoot;
    private bool _showEmptyButtons = true;
    private string? _conditionProcessName;
    private string? _conditionWindowTitle;
    private ButtonViewModel? _selectedButton;

    public string Name
    {
        get => _name;
        set
        {
            if (SetProperty(ref _name, value))
            {
                OnPropertyChanged(nameof(Summary));
            }
        }
    }

    public int Priority
    {
        get => _priority;
        set
        {
            if (SetProperty(ref _priority, value))
            {
                OnPropertyChanged(nameof(Summary));
            }
        }
    }

    public string Theme
    {
        get => _theme;
        set => SetProperty(ref _theme, value);
    }

    public string Color
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

    public string? IconRoot
    {
        get => _iconRoot;
        set => SetProperty(ref _iconRoot, value);
    }

    public bool ShowEmptyButtons
    {
        get => _showEmptyButtons;
        set
        {
            if (SetProperty(ref _showEmptyButtons, value))
            {
                OnPropertyChanged(nameof(EmptyButtonDisplayMode));
            }
        }
    }

    public string EmptyButtonDisplayMode
    {
        get => _showEmptyButtons ? "Theme Icon" : "Black Background";
        set => ShowEmptyButtons = !string.Equals(value, "Black Background", StringComparison.OrdinalIgnoreCase) &&
                                  !string.Equals(value, "Black", StringComparison.OrdinalIgnoreCase);
    }

    public string? ConditionProcessName
    {
        get => _conditionProcessName;
        set
        {
            if (SetProperty(ref _conditionProcessName, value))
            {
                OnPropertyChanged(nameof(HasCondition));
                OnPropertyChanged(nameof(ConditionSummary));
            }
        }
    }

    public string? ConditionWindowTitle
    {
        get => _conditionWindowTitle;
        set
        {
            if (SetProperty(ref _conditionWindowTitle, value))
            {
                OnPropertyChanged(nameof(HasCondition));
                OnPropertyChanged(nameof(ConditionSummary));
            }
        }
    }

    public bool HasCondition => !string.IsNullOrWhiteSpace(ConditionProcessName) || !string.IsNullOrWhiteSpace(ConditionWindowTitle);

    public string ConditionSummary
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(ConditionProcessName) && !string.IsNullOrWhiteSpace(ConditionWindowTitle))
                return $"Process: {ConditionProcessName}, Title: {ConditionWindowTitle}";
            if (!string.IsNullOrWhiteSpace(ConditionProcessName))
                return $"Process: {ConditionProcessName}";
            if (!string.IsNullOrWhiteSpace(ConditionWindowTitle))
                return $"Title: {ConditionWindowTitle}";
            return "Always Active (Fallback)";
        }
    }

    public string Summary => $"{Name} (P:{Priority}, Theme:{Theme})";

    public ObservableCollection<ButtonViewModel> Buttons { get; } = new();

    public ButtonViewModel? SelectedButton
    {
        get => _selectedButton;
        set
        {
            if (_selectedButton != null)
            {
                _selectedButton.IsSelected = false;
            }

            if (SetProperty(ref _selectedButton, value))
            {
                if (_selectedButton != null)
                {
                    _selectedButton.IsSelected = true;
                }
            }
        }
    }

    public LayerViewModel()
    {
        Ensure32Buttons();
    }

    public void Ensure32Buttons()
    {
        while (Buttons.Count < 32)
        {
            Buttons.Add(new ButtonViewModel { Index = Buttons.Count });
        }
    }

    public LayerConfig ToConfig()
    {
        var layerConfig = new LayerConfig
        {
            Name = Name,
            Priority = Priority,
            Theme = string.IsNullOrWhiteSpace(Theme) ? null : Theme,
            Color = Color,
            FontColor = string.IsNullOrWhiteSpace(FontColor) ? null : FontColor,
            Font = string.IsNullOrWhiteSpace(Font) ? null : Font,
            IconRoot = string.IsNullOrWhiteSpace(IconRoot) ? null : IconRoot,
            ShowEmptyButtons = ShowEmptyButtons,
        };

        if (!string.IsNullOrWhiteSpace(ConditionProcessName) || !string.IsNullOrWhiteSpace(ConditionWindowTitle))
        {
            layerConfig.Condition = new ConditionConfig
            {
                ProcessName = string.IsNullOrWhiteSpace(ConditionProcessName) ? null : ConditionProcessName,
                WindowTitle = string.IsNullOrWhiteSpace(ConditionWindowTitle) ? null : ConditionWindowTitle
            };
        }

        var buttonsList = new List<ButtonConfig>();
        foreach (var btn in Buttons.OrderBy(b => b.Index))
        {
            var btnConfig = btn.ToConfig();
            if (btnConfig != null)
            {
                buttonsList.Add(btnConfig);
            }
        }

        layerConfig.Buttons = buttonsList;
        return layerConfig;
    }

    public static LayerViewModel FromConfig(LayerConfig config)
    {
        var vm = new LayerViewModel
        {
            Name = config.Name,
            Priority = config.Priority,
            Theme = config.Theme ?? "default",
            Color = config.Color ?? "#000000",
            FontColor = config.FontColor,
            Font = config.Font,
            IconRoot = config.IconRoot,
            ShowEmptyButtons = config.ShowEmptyButtons,
            ConditionProcessName = config.Condition?.ProcessName,
            ConditionWindowTitle = config.Condition?.WindowTitle
        };

        var buttonMap = (config.Buttons ?? new List<ButtonConfig>()).ToDictionary(b => b.Index);
        vm.Buttons.Clear();

        for (int i = 0; i < 32; i++)
        {
            if (buttonMap.TryGetValue(i, out var btnConfig))
            {
                vm.Buttons.Add(ButtonViewModel.FromConfig(btnConfig));
            }
            else
            {
                vm.Buttons.Add(new ButtonViewModel { Index = i });
            }
        }

        vm.SelectedButton = vm.Buttons.FirstOrDefault();
        return vm;
    }

    public void RefreshPreviews(IGraphicsAdapter graphicsAdapter, string? globalIconRoot)
    {
        var effectiveRoot = !string.IsNullOrWhiteSpace(IconRoot) ? IconRoot : globalIconRoot;
        foreach (var btn in Buttons)
        {
            btn.UpdatePreview(graphicsAdapter, Theme, Color, FontColor, effectiveRoot, ShowEmptyButtons, Font);
        }
    }
}
