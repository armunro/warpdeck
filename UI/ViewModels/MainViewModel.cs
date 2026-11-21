using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;
using Warpdeck.Core.Application;
using Warpdeck.Core.Domain;
using Warpdeck.Infrastructure.Adapters.Yaml;
using Warpdeck.UI.Common;

namespace Warpdeck.UI.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly string _configPath;
    private readonly ILayerProvider _layerProvider;
    private readonly IGraphicsAdapter _graphicsAdapter;
    private readonly IDeviceService _deviceService;
    private readonly LayerManager _layerManager;

    private readonly UndoRedoStack<WorkspaceSnapshot> _undoStack = new(100);
    private readonly UndoRedoStack<WorkspaceSnapshot> _redoStack = new(100);
    private WorkspaceSnapshot? _pendingSnapshot;
    private WorkspaceSnapshot? _currentSnapshot;
    private readonly DispatcherTimer? _debounceTimer;
    private bool _isRestoringState;
    private bool _isLoadingConfig;
    private ButtonViewModel? _subscribedButton;

    private string? _iconRoot;
    private int _brightness = 100;
    private LayerViewModel? _selectedLayer;
    private string _statusMessage = "Ready";
    private bool _isDeviceConnected;
    private bool _isRecordingKeystrokes;
    private bool _appendRecordedKeystrokes = true;
    private string _recordedShortcutPreview = "Press keys to record...";
    private ButtonConfig? _buttonClipboard;

    public ObservableCollection<LayerViewModel> Layers { get; } = new();

    public IReadOnlyList<string> AvailableThemes { get; } = new[] { "cyber", "synthwave", "korry", "default" };
    public IReadOnlyList<string> AvailableActionTypes { get; } = new[] { "macro", "brightness", "none" };
    public IReadOnlyList<string> AvailableEmptyButtonModes { get; } = new[] { "Theme Icon", "Black Background" };
    public IReadOnlyList<string> AvailableFonts { get; } = new[] { "(Default)", "Arial", "Segoe UI", "Consolas", "Impact", "Trebuchet MS", "Verdana", "Tahoma", "Courier New", "Georgia", "Lucida Console", "Calibri" };

    public bool HasClipboardButton => _buttonClipboard != null;

    public string ClipboardButtonTitle => _buttonClipboard != null
        ? (!string.IsNullOrWhiteSpace(_buttonClipboard.Caption) ? _buttonClipboard.Caption : (!string.IsNullOrWhiteSpace(_buttonClipboard.Icon) ? Path.GetFileNameWithoutExtension(_buttonClipboard.Icon) : $"Button #{_buttonClipboard.Index}"))
        : "Empty";

    public IEnumerable<LayerViewModel> OtherLayers => Layers.Where(l => l != SelectedLayer);

    public bool CanUndo => _undoStack.Count > 0 || _pendingSnapshot != null;
    public bool CanRedo => _redoStack.Count > 0;

    public string? IconRoot
    {
        get => _iconRoot;
        set
        {
            if (SetProperty(ref _iconRoot, value))
            {
                _graphicsAdapter.SetIconRoot(value);
                RefreshCurrentLayerPreviews();
                if (!_isRestoringState && !_isLoadingConfig)
                {
                    NotifyStateChanged("Change Global Icon Root");
                }
            }
        }
    }

    public int Brightness
    {
        get => _brightness;
        set
        {
            int clamped = Math.Clamp(value, 0, 100);
            if (SetProperty(ref _brightness, clamped))
            {
                _deviceService.SetBrightness((byte)clamped);
            }
        }
    }

    public LayerViewModel? SelectedLayer
    {
        get => _selectedLayer;
        set
        {
            if (_selectedLayer != null)
            {
                _selectedLayer.PropertyChanged -= SelectedLayer_PropertyChanged;
            }

            if (SetProperty(ref _selectedLayer, value))
            {
                if (_selectedLayer != null)
                {
                    _selectedLayer.PropertyChanged += SelectedLayer_PropertyChanged;
                }

                IsRecordingKeystrokes = false;
                UpdateButtonSubscription();
                OnPropertyChanged(nameof(SelectedButton));
                OnPropertyChanged(nameof(OtherLayers));
                RefreshCurrentLayerPreviews();
            }
        }
    }

    private void SelectedLayer_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LayerViewModel.SelectedButton))
        {
            UpdateButtonSubscription();
            OnPropertyChanged(nameof(SelectedButton));
            RefreshCurrentButtonPreview();
        }
        else if (e.PropertyName is nameof(LayerViewModel.Theme) or
                                  nameof(LayerViewModel.Color) or
                                  nameof(LayerViewModel.FontColor) or
                                  nameof(LayerViewModel.Font) or
                                  nameof(LayerViewModel.IconRoot) or
                                  nameof(LayerViewModel.ShowEmptyButtons) or
                                  nameof(LayerViewModel.EmptyButtonDisplayMode))
        {
            RefreshCurrentLayerPreviews();
            if (!_isRestoringState && !_isLoadingConfig)
            {
                NotifyStateChanged($"Edit Layer '{SelectedLayer?.Name}' Settings");
            }
        }
        else if (e.PropertyName is nameof(LayerViewModel.Name) or
                                  nameof(LayerViewModel.Priority) or
                                  nameof(LayerViewModel.ConditionProcessName) or
                                  nameof(LayerViewModel.ConditionWindowTitle))
        {
            if (!_isRestoringState && !_isLoadingConfig)
            {
                NotifyStateChanged($"Edit Layer '{SelectedLayer?.Name}' Properties");
            }
        }
    }

    private void UpdateButtonSubscription()
    {
        if (_subscribedButton != null)
        {
            _subscribedButton.PropertyChanged -= SelectedButton_PropertyChanged;
        }

        _subscribedButton = SelectedButton;

        if (_subscribedButton != null)
        {
            _subscribedButton.PropertyChanged += SelectedButton_PropertyChanged;
        }
    }

    private void SelectedButton_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (_isRestoringState || _isLoadingConfig) return;

        if (e.PropertyName is nameof(ButtonViewModel.Caption) or
                              nameof(ButtonViewModel.Icon) or
                              nameof(ButtonViewModel.Theme) or
                              nameof(ButtonViewModel.Color) or
                              nameof(ButtonViewModel.FontColor) or
                              nameof(ButtonViewModel.Font) or
                              nameof(ButtonViewModel.ActionType) or
                              nameof(ButtonViewModel.ActionValue))
        {
            RefreshCurrentButtonPreview();
            NotifyStateChanged($"Edit Button #{_subscribedButton?.Index}");
        }
        else if (e.PropertyName is nameof(ButtonViewModel.IsLocked))
        {
            RefreshCurrentLayerPreviews();
            NotifyStateChanged($"Toggle Lock Button #{_subscribedButton?.Index}");
        }
    }

    public ButtonViewModel? SelectedButton => SelectedLayer?.SelectedButton;

    public bool IsRecordingKeystrokes
    {
        get => _isRecordingKeystrokes;
        set
        {
            if (SetProperty(ref _isRecordingKeystrokes, value))
            {
                OnPropertyChanged(nameof(RecordStatusText));
                if (value)
                {
                    StatusMessage = "Recording keystrokes... Press keys or shortcuts (Esc to stop).";
                }
            }
        }
    }

    public bool AppendRecordedKeystrokes
    {
        get => _appendRecordedKeystrokes;
        set => SetProperty(ref _appendRecordedKeystrokes, value);
    }

    public string RecordedShortcutPreview
    {
        get => _recordedShortcutPreview;
        set
        {
            if (SetProperty(ref _recordedShortcutPreview, value))
            {
                OnPropertyChanged(nameof(RecordStatusText));
            }
        }
    }

    public string RecordStatusText => IsRecordingKeystrokes
        ? $"Recording: {_recordedShortcutPreview}"
        : "Ready to record";

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public bool IsDeviceConnected
    {
        get => _isDeviceConnected;
        set => SetProperty(ref _isDeviceConnected, value);
    }

    // Commands
    public ICommand UndoCommand { get; }
    public ICommand RedoCommand { get; }
    public ICommand DeleteButtonCommand { get; }
    public ICommand CopyButtonCommand { get; }
    public ICommand PasteButtonCommand { get; }
    public ICommand CopyButtonToLayerCommand { get; }
    public ICommand AddLayerCommand { get; }
    public ICommand RemoveLayerCommand { get; }
    public ICommand DuplicateLayerCommand { get; }
    public ICommand MoveLayerUpCommand { get; }
    public ICommand MoveLayerDownCommand { get; }
    public ICommand SaveConfigCommand { get; }
    public ICommand ReloadConfigCommand { get; }
    public ICommand RefreshPreviewsCommand { get; }
    public ICommand BrowseIconRootCommand { get; }
    public ICommand BrowseButtonIconCommand { get; }
    public ICommand ClearButtonCommand { get; }
    public ICommand SelectButtonCommand { get; }
    public ICommand InsertMacroCommand { get; }
    public ICommand ToggleRecordKeystrokesCommand { get; }
    public ICommand BackspaceMacroCommand { get; }
    public ICommand ClearMacroCommand { get; }

    public MainViewModel(
        ILayerProvider layerProvider,
        IGraphicsAdapter graphicsAdapter,
        IDeviceService deviceService,
        LayerManager layerManager,
        string configPath = "config.yaml")
    {
        _layerProvider = layerProvider;
        _graphicsAdapter = graphicsAdapter;
        _deviceService = deviceService;
        _layerManager = layerManager;
        _configPath = Path.GetFullPath(configPath);

        if (System.Windows.Application.Current != null)
        {
            _debounceTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _debounceTimer.Tick += (s, e) => CommitPendingSnapshot();
        }

        // Initialize Commands
        UndoCommand = new RelayCommand(Undo, () => CanUndo);
        RedoCommand = new RelayCommand(Redo, () => CanRedo);
        DeleteButtonCommand = new RelayCommand(p => DeleteSelectedButton(p as ButtonViewModel), _ => SelectedButton != null);
        ClearButtonCommand = new RelayCommand(p => DeleteSelectedButton(p as ButtonViewModel), _ => SelectedButton != null);

        CopyButtonCommand = new RelayCommand(p =>
        {
            var btn = p as ButtonViewModel ?? SelectedButton;
            if (btn != null) CopyButton(btn);
        }, _ => SelectedButton != null);

        PasteButtonCommand = new RelayCommand(p =>
        {
            var btn = p as ButtonViewModel ?? SelectedButton;
            if (btn != null) PasteButton(btn);
        }, _ => SelectedButton != null && HasClipboardButton);

        CopyButtonToLayerCommand = new RelayCommand(p =>
        {
            if (SelectedButton == null) return;
            LayerViewModel? targetLayer = null;
            if (p is LayerViewModel lvm)
            {
                targetLayer = lvm;
            }
            else if (p is string layerName)
            {
                targetLayer = Layers.FirstOrDefault(l => string.Equals(l.Name, layerName, StringComparison.OrdinalIgnoreCase));
            }

            if (targetLayer != null)
            {
                CopyButtonToLayer(SelectedButton, targetLayer);
            }
        }, _ => SelectedButton != null);

        AddLayerCommand = new RelayCommand(AddLayer);
        RemoveLayerCommand = new RelayCommand(RemoveLayer, () => SelectedLayer != null && Layers.Count > 1);
        DuplicateLayerCommand = new RelayCommand(DuplicateLayer, () => SelectedLayer != null);
        MoveLayerUpCommand = new RelayCommand(MoveLayerUp, () => SelectedLayer != null && Layers.IndexOf(SelectedLayer) > 0);
        MoveLayerDownCommand = new RelayCommand(MoveLayerDown, () => SelectedLayer != null && Layers.IndexOf(SelectedLayer) < Layers.Count - 1);
        SaveConfigCommand = new RelayCommand(SaveConfig);
        ReloadConfigCommand = new RelayCommand(ReloadConfig);
        RefreshPreviewsCommand = new RelayCommand(RefreshCurrentLayerPreviews);
        BrowseIconRootCommand = new RelayCommand(BrowseIconRoot);
        BrowseButtonIconCommand = new RelayCommand(BrowseButtonIcon, () => SelectedButton != null);
        SelectButtonCommand = new RelayCommand(p =>
        {
            if (p is ButtonViewModel btn && SelectedLayer != null)
            {
                IsRecordingKeystrokes = false;
                SelectedLayer.SelectedButton = btn;
                UpdateButtonSubscription();
                OnPropertyChanged(nameof(SelectedButton));
            }
        });
        InsertMacroCommand = new RelayCommand(p =>
        {
            if (p is string macro && SelectedButton != null)
            {
                RecordSnapshot($"Insert Macro Preset for Button #{SelectedButton.Index}");
                SelectedButton.ActionType = "macro";
                SelectedButton.ActionValue = macro;
                _currentSnapshot = CreateSnapshot($"Insert Macro Preset for Button #{SelectedButton.Index}");
                RefreshCurrentButtonPreview();
            }
        });
        ToggleRecordKeystrokesCommand = new RelayCommand(() =>
        {
            if (SelectedButton == null) return;
            IsRecordingKeystrokes = !IsRecordingKeystrokes;
            if (IsRecordingKeystrokes)
            {
                RecordedShortcutPreview = "Press keys...";
                StatusMessage = "🔴 Recording keystrokes... Press shortcut keys on keyboard (Esc to stop).";
            }
            else
            {
                StatusMessage = "Keystroke recording stopped.";
            }
        }, () => SelectedButton != null);

        BackspaceMacroCommand = new RelayCommand(BackspaceMacro, () => SelectedButton != null);
        ClearMacroCommand = new RelayCommand(ClearMacro, () => SelectedButton != null);

        Layers.CollectionChanged += (s, e) => OnPropertyChanged(nameof(OtherLayers));

        // Device Connection Events
        _deviceService.DeviceConnected += _ =>
        {
            System.Windows.Application.Current?.Dispatcher.Invoke(() =>
            {
                IsDeviceConnected = true;
                StatusMessage = "Stream Deck connected.";
            });
        };
        _deviceService.DeviceDisconnected += () =>
        {
            System.Windows.Application.Current?.Dispatcher.Invoke(() =>
            {
                IsDeviceConnected = false;
                StatusMessage = "Stream Deck disconnected. Waiting for device...";
            });
        };
        IsDeviceConnected = _deviceService.CurrentDevice != null;

        LoadFromConfig();
    }

    public void LoadFromConfig()
    {
        _isLoadingConfig = true;
        try
        {
            var config = ConfigLoader.Load(_configPath);
            _iconRoot = config.IconRoot;
            OnPropertyChanged(nameof(IconRoot));
            _graphicsAdapter.SetIconRoot(_iconRoot);

            if (config.Brightness.HasValue)
            {
                _brightness = Math.Clamp((int)config.Brightness.Value, 0, 100);
                OnPropertyChanged(nameof(Brightness));
                _deviceService.SetBrightness((byte)_brightness);
            }

            if (_selectedLayer != null)
            {
                _selectedLayer.PropertyChanged -= SelectedLayer_PropertyChanged;
            }
            if (_subscribedButton != null)
            {
                _subscribedButton.PropertyChanged -= SelectedButton_PropertyChanged;
                _subscribedButton = null;
            }

            Layers.Clear();
            foreach (var layerConfig in config.Layers)
            {
                var layerVm = LayerViewModel.FromConfig(layerConfig);
                Layers.Add(layerVm);
            }

            if (Layers.Count == 0)
            {
                var defaultLayer = new LayerViewModel { Name = "Default", Priority = 0, Theme = "cyber" };
                Layers.Add(defaultLayer);
            }

            SelectedLayer = Layers.FirstOrDefault();
            UpdateButtonSubscription();

            _undoStack.Clear();
            _redoStack.Clear();
            _pendingSnapshot = null;
            _currentSnapshot = CreateSnapshot("Initial State");
            UpdateUndoRedoState();

            StatusMessage = $"Loaded {Layers.Count} layer(s) from config.yaml";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading config: {ex.Message}";
        }
        finally
        {
            _isLoadingConfig = false;
        }
    }

    public WorkspaceSnapshot CreateSnapshot(string description = "")
    {
        int selectedLayerIndex = SelectedLayer != null ? Layers.IndexOf(SelectedLayer) : 0;
        int? selectedButtonIndex = SelectedButton?.Index;

        return new WorkspaceSnapshot
        {
            Layers = Layers.Select(l => WorkspaceSnapshot.CloneLayer(l.ToConfig())).ToList(),
            IconRoot = IconRoot,
            Brightness = Brightness,
            SelectedLayerIndex = selectedLayerIndex >= 0 ? selectedLayerIndex : 0,
            SelectedButtonIndex = selectedButtonIndex,
            Description = description
        };
    }

    public void RecordSnapshot(string description)
    {
        if (_isRestoringState || _isLoadingConfig) return;

        CommitPendingSnapshot();

        if (_currentSnapshot != null)
        {
            _undoStack.Push(_currentSnapshot);
            _redoStack.Clear();
            UpdateUndoRedoState();
        }

        _currentSnapshot = CreateSnapshot(description);
    }

    public void CommitPendingSnapshot()
    {
        _debounceTimer?.Stop();
        if (_pendingSnapshot != null)
        {
            _undoStack.Push(_pendingSnapshot);
            _redoStack.Clear();
            _pendingSnapshot = null;
            UpdateUndoRedoState();
        }
    }

    public void NotifyStateChanged(string description)
    {
        if (_isRestoringState || _isLoadingConfig) return;

        if (_pendingSnapshot == null)
        {
            _pendingSnapshot = _currentSnapshot ?? CreateSnapshot(description);
        }

        _currentSnapshot = CreateSnapshot(description);

        if (_debounceTimer != null)
        {
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }
        else
        {
            CommitPendingSnapshot();
        }

        UpdateUndoRedoState();
    }

    public void Undo()
    {
        CommitPendingSnapshot();

        if (_undoStack.Count == 0) return;

        var snapshotToRestore = _undoStack.Pop();
        if (_currentSnapshot != null)
        {
            _redoStack.Push(_currentSnapshot);
        }

        _isRestoringState = true;
        try
        {
            RestoreSnapshot(snapshotToRestore);
            _currentSnapshot = snapshotToRestore;
            StatusMessage = string.IsNullOrWhiteSpace(snapshotToRestore.Description)
                ? "Undo performed"
                : $"Undo: {snapshotToRestore.Description}";
        }
        finally
        {
            _isRestoringState = false;
            UpdateUndoRedoState();
        }
    }

    public void Redo()
    {
        CommitPendingSnapshot();

        if (_redoStack.Count == 0) return;

        var snapshotToRestore = _redoStack.Pop();
        if (_currentSnapshot != null)
        {
            _undoStack.Push(_currentSnapshot);
        }

        _isRestoringState = true;
        try
        {
            RestoreSnapshot(snapshotToRestore);
            _currentSnapshot = snapshotToRestore;
            StatusMessage = string.IsNullOrWhiteSpace(snapshotToRestore.Description)
                ? "Redo performed"
                : $"Redo: {snapshotToRestore.Description}";
        }
        finally
        {
            _isRestoringState = false;
            UpdateUndoRedoState();
        }
    }

    public void RestoreSnapshot(WorkspaceSnapshot snapshot)
    {
        if (snapshot == null) return;

        _isRestoringState = true;
        try
        {
            if (snapshot.IconRoot != _iconRoot)
            {
                _iconRoot = snapshot.IconRoot;
                OnPropertyChanged(nameof(IconRoot));
                _graphicsAdapter.SetIconRoot(_iconRoot);
            }

            if (snapshot.Brightness != _brightness)
            {
                _brightness = snapshot.Brightness;
                OnPropertyChanged(nameof(Brightness));
                _deviceService.SetBrightness((byte)_brightness);
            }

            if (_selectedLayer != null)
            {
                _selectedLayer.PropertyChanged -= SelectedLayer_PropertyChanged;
            }
            if (_subscribedButton != null)
            {
                _subscribedButton.PropertyChanged -= SelectedButton_PropertyChanged;
                _subscribedButton = null;
            }

            Layers.Clear();
            foreach (var layerConfig in snapshot.Layers)
            {
                var layerVm = LayerViewModel.FromConfig(WorkspaceSnapshot.CloneLayer(layerConfig));
                Layers.Add(layerVm);
            }

            if (Layers.Count == 0)
            {
                var defaultLayer = new LayerViewModel { Name = "Default", Priority = 0, Theme = "cyber" };
                Layers.Add(defaultLayer);
            }

            int layerIndex = Math.Clamp(snapshot.SelectedLayerIndex, 0, Layers.Count - 1);
            SelectedLayer = Layers[layerIndex];

            if (snapshot.SelectedButtonIndex.HasValue && SelectedLayer != null)
            {
                var btn = SelectedLayer.Buttons.FirstOrDefault(b => b.Index == snapshot.SelectedButtonIndex.Value);
                if (btn != null)
                {
                    SelectedLayer.SelectedButton = btn;
                }
            }

            UpdateButtonSubscription();
            OnPropertyChanged(nameof(SelectedButton));
            RefreshCurrentLayerPreviews();
        }
        finally
        {
            _isRestoringState = false;
        }
    }

    private void UpdateUndoRedoState()
    {
        OnPropertyChanged(nameof(CanUndo));
        OnPropertyChanged(nameof(CanRedo));
        if (UndoCommand is RelayCommand undoCmd) undoCmd.RaiseCanExecuteChanged();
        if (RedoCommand is RelayCommand redoCmd) redoCmd.RaiseCanExecuteChanged();
    }

    public void SaveConfig()
    {
        try
        {
            var appConfig = new AppConfig
            {
                IconRoot = string.IsNullOrWhiteSpace(IconRoot) ? null : IconRoot,
                Brightness = (byte)Brightness,
                Layers = Layers.Select(l => l.ToConfig()).ToList()
            };

            ConfigLoader.Save(_configPath, appConfig);

            // Update live layers on connected device
            _graphicsAdapter.SetIconRoot(appConfig.IconRoot);
            _deviceService.SetBrightness((byte)Brightness);
            _layerManager.SetLayers(_layerProvider.GetLayers());

            StatusMessage = $"Saved {Layers.Count} layers to config.yaml at {DateTime.Now:HH:mm:ss}";
            RefreshCurrentLayerPreviews();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error saving config: {ex.Message}";
        }
    }

    private void ReloadConfig()
    {
        LoadFromConfig();
        RefreshCurrentLayerPreviews();
        StatusMessage = "Reloaded configuration from disk.";
    }

    private void AddLayer()
    {
        RecordSnapshot("Add Layer");
        int highestPriority = Layers.Count > 0 ? Layers.Max(l => l.Priority) + 10 : 0;
        var newLayer = new LayerViewModel
        {
            Name = $"Layer {Layers.Count + 1}",
            Priority = highestPriority,
            Theme = "cyber",
            Color = "#000000"
        };
        Layers.Add(newLayer);
        SelectedLayer = newLayer;
        _currentSnapshot = CreateSnapshot($"Add Layer '{newLayer.Name}'");
        StatusMessage = $"Added new layer '{newLayer.Name}'";
    }

    private void RemoveLayer()
    {
        if (SelectedLayer == null || Layers.Count <= 1) return;

        var layerToRemove = SelectedLayer;
        RecordSnapshot($"Remove Layer '{layerToRemove.Name}'");
        int index = Layers.IndexOf(layerToRemove);
        Layers.Remove(layerToRemove);

        if (index >= Layers.Count) index = Layers.Count - 1;
        SelectedLayer = Layers[index];
        _currentSnapshot = CreateSnapshot($"Remove Layer '{layerToRemove.Name}'");
        StatusMessage = $"Removed layer '{layerToRemove.Name}'";
    }

    private void DuplicateLayer()
    {
        if (SelectedLayer == null) return;

        RecordSnapshot($"Duplicate Layer '{SelectedLayer.Name}'");
        var config = SelectedLayer.ToConfig();
        config.Name = $"{config.Name} (Copy)";
        config.Priority += 5;

        var duplicate = LayerViewModel.FromConfig(config);
        Layers.Add(duplicate);
        SelectedLayer = duplicate;
        _currentSnapshot = CreateSnapshot($"Duplicate Layer '{duplicate.Name}'");
        StatusMessage = $"Duplicated layer as '{duplicate.Name}'";
    }

    private void MoveLayerUp()
    {
        if (SelectedLayer == null) return;
        int index = Layers.IndexOf(SelectedLayer);
        if (index > 0)
        {
            RecordSnapshot($"Move Layer '{SelectedLayer.Name}' Up");
            Layers.Move(index, index - 1);
            _currentSnapshot = CreateSnapshot($"Move Layer '{SelectedLayer.Name}' Up");
            StatusMessage = $"Moved layer '{SelectedLayer.Name}' up";
        }
    }

    private void MoveLayerDown()
    {
        if (SelectedLayer == null) return;
        int index = Layers.IndexOf(SelectedLayer);
        if (index < Layers.Count - 1)
        {
            RecordSnapshot($"Move Layer '{SelectedLayer.Name}' Down");
            Layers.Move(index, index + 1);
            _currentSnapshot = CreateSnapshot($"Move Layer '{SelectedLayer.Name}' Down");
            StatusMessage = $"Moved layer '{SelectedLayer.Name}' down";
        }
    }

    private void BrowseIconRoot()
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Select Icons Root Directory",
            InitialDirectory = !string.IsNullOrWhiteSpace(IconRoot) && Directory.Exists(IconRoot) ? IconRoot : @"C:\Tools"
        };

        if (dialog.ShowDialog() == true)
        {
            IconRoot = dialog.FolderName;
            StatusMessage = $"Updated icon root directory to '{IconRoot}'";
        }
    }

    private void BrowseButtonIcon()
    {
        if (SelectedButton == null) return;

        var initialDir = !string.IsNullOrWhiteSpace(IconRoot) && Directory.Exists(IconRoot) 
            ? IconRoot 
            : AppContext.BaseDirectory;

        var dialog = new OpenFileDialog
        {
            Title = "Select Icon (SVG / PNG)",
            Filter = "Vector & Image Files (*.svg;*.png)|*.svg;*.png|SVG Files (*.svg)|*.svg|All Files (*.*)|*.*",
            InitialDirectory = initialDir
        };

        if (dialog.ShowDialog() == true)
        {
            RecordSnapshot($"Change Icon for Button #{SelectedButton.Index}");
            var selectedFile = dialog.FileName;
            // If inside IconRoot, make relative
            if (!string.IsNullOrWhiteSpace(IconRoot) && selectedFile.StartsWith(IconRoot, StringComparison.OrdinalIgnoreCase))
            {
                var rel = Path.GetRelativePath(IconRoot, selectedFile);
                SelectedButton.Icon = rel.Replace('\\', '/');
            }
            else
            {
                SelectedButton.Icon = selectedFile;
            }

            _currentSnapshot = CreateSnapshot($"Change Icon for Button #{SelectedButton.Index}");
            RefreshCurrentButtonPreview();
            StatusMessage = $"Selected icon: {SelectedButton.Icon}";
        }
    }

    public void DeleteSelectedButton(ButtonViewModel? button = null)
    {
        var btn = button ?? SelectedButton;
        if (btn == null) return;

        RecordSnapshot($"Delete Button #{btn.Index}");
        int index = btn.Index;
        btn.Clear();
        if (btn == SelectedButton)
        {
            RefreshCurrentButtonPreview();
        }
        else if (SelectedLayer != null)
        {
            var effectiveRoot = !string.IsNullOrWhiteSpace(SelectedLayer.IconRoot) ? SelectedLayer.IconRoot : IconRoot;
            btn.UpdatePreview(_graphicsAdapter, SelectedLayer.Theme, SelectedLayer.Color, SelectedLayer.FontColor, effectiveRoot, SelectedLayer.ShowEmptyButtons, SelectedLayer.Font);
        }
        _currentSnapshot = CreateSnapshot($"Delete Button #{index}");
        StatusMessage = $"Deleted button {index}";
    }

    public void CopyButton(ButtonViewModel? button = null)
    {
        var btn = button ?? SelectedButton;
        if (btn == null) return;

        _buttonClipboard = btn.ToConfig() ?? new ButtonConfig { Index = btn.Index };
        OnPropertyChanged(nameof(HasClipboardButton));
        OnPropertyChanged(nameof(ClipboardButtonTitle));
        if (PasteButtonCommand is RelayCommand pasteCmd) pasteCmd.RaiseCanExecuteChanged();
        StatusMessage = $"Copied '{btn.DisplayTitle}' (Button #{btn.Index}) to clipboard.";
    }

    public void PasteButton(ButtonViewModel? targetButton = null)
    {
        var target = targetButton ?? SelectedButton;
        if (target == null || _buttonClipboard == null) return;

        string layerName = SelectedLayer?.Name ?? "Layer";
        RecordSnapshot($"Paste Button #{target.Index} on {layerName}");

        target.Caption = _buttonClipboard.Caption;
        target.Icon = _buttonClipboard.Icon;
        target.Theme = _buttonClipboard.Theme;
        target.Color = _buttonClipboard.Color;
        target.FontColor = _buttonClipboard.FontColor;
        target.Font = _buttonClipboard.Font;
        target.ActionType = _buttonClipboard.Action?.Type ?? "macro";
        target.ActionValue = _buttonClipboard.Action?.Value ?? string.Empty;
        target.IsLocked = _buttonClipboard.Locked;

        _currentSnapshot = CreateSnapshot($"Paste Button #{target.Index} on {layerName}");
        RefreshCurrentButtonPreview();
        StatusMessage = $"Pasted '{target.DisplayTitle}' to Button #{target.Index}.";
    }

    public void CopyButtonToLayer(ButtonViewModel sourceButton, LayerViewModel targetLayer, int? targetIndex = null)
    {
        if (sourceButton == null || targetLayer == null) return;

        int destIndex = targetIndex ?? sourceButton.Index;
        var targetButton = targetLayer.Buttons.FirstOrDefault(b => b.Index == destIndex);
        if (targetButton == null) return;

        RecordSnapshot($"Copy Button #{sourceButton.Index} to Layer '{targetLayer.Name}'");
        targetButton.CopyFrom(sourceButton);
        _currentSnapshot = CreateSnapshot($"Copy Button #{sourceButton.Index} to Layer '{targetLayer.Name}'");
        var effectiveRoot = !string.IsNullOrWhiteSpace(targetLayer.IconRoot) ? targetLayer.IconRoot : IconRoot;
        targetLayer.RefreshPreviews(_graphicsAdapter, effectiveRoot);

        // Also save to clipboard buffer
        _buttonClipboard = sourceButton.ToConfig() ?? new ButtonConfig { Index = sourceButton.Index };
        OnPropertyChanged(nameof(HasClipboardButton));
        OnPropertyChanged(nameof(ClipboardButtonTitle));
        if (PasteButtonCommand is RelayCommand pasteCmd) pasteCmd.RaiseCanExecuteChanged();

        StatusMessage = $"Copied Button #{sourceButton.Index} ('{sourceButton.DisplayTitle}') to Layer '{targetLayer.Name}' (Slot #{destIndex}).";
    }

    public void MoveOrSwapButton(ButtonViewModel source, ButtonViewModel target, bool isCopy)
    {
        if (source == null || target == null || source == target) return;

        string desc = isCopy
            ? $"Copy Button #{source.Index} to #{target.Index}"
            : $"Move/Swap Button #{source.Index} to #{target.Index}";

        RecordSnapshot(desc);

        if (isCopy)
        {
            target.CopyFrom(source);
            StatusMessage = $"Copied Button #{source.Index} to #{target.Index}";
        }
        else
        {
            bool targetHadContent = target.HasContent || target.IsLocked;
            source.SwapWith(target);
            if (targetHadContent)
            {
                StatusMessage = $"Swapped Button #{source.Index} and #{target.Index}";
            }
            else
            {
                StatusMessage = $"Moved Button #{source.Index} to #{target.Index}";
            }
        }

        if (SelectedLayer != null)
        {
            SelectedLayer.SelectedButton = target;
            UpdateButtonSubscription();
        }
        _currentSnapshot = CreateSnapshot(desc);
        RefreshCurrentLayerPreviews();
    }

    public void RefreshCurrentLayerPreviews()
    {
        if (SelectedLayer == null) return;

        // Check if any other layer (such as Default layer) has locked buttons on slots
        // Find all locked buttons in base/default or other layers
        var lockedByOtherLayer = new Dictionary<int, string>();
        foreach (var layer in Layers)
        {
            if (layer == SelectedLayer) continue;
            foreach (var btn in layer.Buttons)
            {
                if (btn.IsLocked)
                {
                    lockedByOtherLayer[btn.Index] = layer.Name;
                }
            }
        }

        foreach (var btn in SelectedLayer.Buttons)
        {
            if (lockedByOtherLayer.TryGetValue(btn.Index, out var lockedLayerName) && !btn.IsLocked)
            {
                btn.IsBlockedByLockedLayer = true;
                btn.BlockedMessage = $"Locked by '{lockedLayerName}' layer. Cannot be overridden.";
            }
            else
            {
                btn.IsBlockedByLockedLayer = false;
                btn.BlockedMessage = null;
            }
        }

        SelectedLayer.RefreshPreviews(_graphicsAdapter, IconRoot);
    }

    public void RefreshCurrentButtonPreview()
    {
        if (SelectedLayer == null || SelectedButton == null) return;
        var effectiveRoot = !string.IsNullOrWhiteSpace(SelectedLayer.IconRoot) ? SelectedLayer.IconRoot : IconRoot;
        SelectedButton.UpdatePreview(_graphicsAdapter, SelectedLayer.Theme, SelectedLayer.Color, SelectedLayer.FontColor, effectiveRoot, SelectedLayer.ShowEmptyButtons, SelectedLayer.Font);
    }

    public void RecordKeystroke(Key key, ModifierKeys modifiers)
    {
        if (SelectedButton == null) return;

        if (key == Key.Escape && modifiers == ModifierKeys.None)
        {
            IsRecordingKeystrokes = false;
            StatusMessage = "Keystroke recording stopped.";
            return;
        }

        var macro = KeyMacroHelper.ConvertToMacro(key, modifiers);
        if (macro != null)
        {
            RecordSnapshot($"Record Shortcut for Button #{SelectedButton.Index}");
            SelectedButton.ActionType = "macro";
            if (AppendRecordedKeystrokes && !string.IsNullOrWhiteSpace(SelectedButton.ActionValue))
            {
                SelectedButton.ActionValue += macro;
            }
            else
            {
                SelectedButton.ActionValue = macro;
            }

            RecordedShortcutPreview = KeyMacroHelper.FormatShortcutPreview(key, modifiers);
            StatusMessage = $"Recorded: {RecordedShortcutPreview} ({macro})";
            _currentSnapshot = CreateSnapshot($"Record Shortcut for Button #{SelectedButton.Index}");
            RefreshCurrentButtonPreview();
        }
    }

    public void UpdateKeyPreview(Key key, ModifierKeys modifiers)
    {
        if (!IsRecordingKeystrokes) return;
        RecordedShortcutPreview = KeyMacroHelper.FormatShortcutPreview(key, modifiers);
    }

    public void BackspaceMacro()
    {
        if (SelectedButton == null || string.IsNullOrEmpty(SelectedButton.ActionValue)) return;

        RecordSnapshot($"Backspace Macro for Button #{SelectedButton.Index}");
        var val = SelectedButton.ActionValue;
        if (val.EndsWith("}}"))
        {
            int lastOpen = val.LastIndexOf("{{", StringComparison.Ordinal);
            if (lastOpen >= 0)
            {
                SelectedButton.ActionValue = val.Substring(0, lastOpen);
            }
            else
            {
                SelectedButton.ActionValue = val.Substring(0, val.Length - 1);
            }
        }
        else
        {
            SelectedButton.ActionValue = val.Substring(0, val.Length - 1);
        }

        _currentSnapshot = CreateSnapshot($"Backspace Macro for Button #{SelectedButton.Index}");
        RefreshCurrentButtonPreview();
        StatusMessage = "Removed last macro element.";
    }

    public void ClearMacro()
    {
        if (SelectedButton == null) return;
        RecordSnapshot($"Clear Macro for Button #{SelectedButton.Index}");
        SelectedButton.ActionValue = string.Empty;
        _currentSnapshot = CreateSnapshot($"Clear Macro for Button #{SelectedButton.Index}");
        RefreshCurrentButtonPreview();
        StatusMessage = "Cleared macro value.";
    }
}
