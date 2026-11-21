using System;
using System.IO;
using System.Windows.Input;
using OpenMacroBoard.SDK;
using Warpdeck.Core.Application;
using Warpdeck.Core.Domain;
using Warpdeck.Infrastructure.Adapters.Graphics;
using Warpdeck.Infrastructure.Adapters.Windows;
using Warpdeck.Infrastructure.Adapters.Yaml;
using Warpdeck.UI.Common;
using Warpdeck.UI.ViewModels;

namespace TestNamespace;

public class MockDeviceService : IDeviceService
{
    public byte Brightness { get; private set; } = 100;
    public IMacroBoard? CurrentDevice => null;
    public event Action<IMacroBoard>? DeviceConnected;
    public event Action? DeviceDisconnected;
    public void Start() {}
    public void Stop() {}
    public void SetBrightness(byte percent) => Brightness = percent;
}

public static class TestRecording
{
    public static void RunTests()
    {
        Console.WriteLine("=== Testing Key Recording and Modifier Handling ===");

        // 1. Test KeyMacroHelper.ConvertToMacro
        AssertEqual(KeyMacroHelper.ConvertToMacro(Key.None, ModifierKeys.Control | ModifierKeys.Alt), "{{ctrl+alt}}", "Ctrl+Alt macro");
        AssertEqual(KeyMacroHelper.ConvertToMacro(Key.D1, ModifierKeys.Control | ModifierKeys.Alt), "{{ctrl+alt+1}}", "Ctrl+Alt+1 macro");
        AssertEqual(KeyMacroHelper.ConvertToMacro(Key.W, ModifierKeys.Control | ModifierKeys.Alt), "{{ctrl+alt+w}}", "Ctrl+Alt+W macro");
        AssertEqual(KeyMacroHelper.ConvertToMacro(Key.Right, ModifierKeys.Windows | ModifierKeys.Shift), "{{win+shift+right}}", "Win+Shift+Right macro");
        AssertEqual(KeyMacroHelper.ConvertToMacro(Key.None, ModifierKeys.Control), "{{ctrl}}", "Ctrl alone macro");
        AssertEqual(KeyMacroHelper.ConvertToMacro(Key.None, ModifierKeys.Alt), "{{alt}}", "Alt alone macro");
        AssertEqual(KeyMacroHelper.ConvertToMacro(Key.None, ModifierKeys.Shift), "{{shift}}", "Shift alone macro");
        AssertEqual(KeyMacroHelper.ConvertToMacro(Key.None, ModifierKeys.Windows), "{{win}}", "Win alone macro");
        AssertEqual(KeyMacroHelper.ConvertToMacro(Key.LeftCtrl, ModifierKeys.None), null, "LeftCtrl raw key should return null");
        AssertEqual(KeyMacroHelper.ConvertToMacro(Key.LeftAlt, ModifierKeys.None), null, "LeftAlt raw key should return null");
        AssertEqual(KeyMacroHelper.ConvertToMacro(Key.None, ModifierKeys.None), null, "None should return null");

        Console.WriteLine("[PASS] KeyMacroHelper.ConvertToMacro tests");

        // 2. Test KeyMacroHelper.FormatShortcutPreview
        AssertEqual(KeyMacroHelper.FormatShortcutPreview(Key.None, ModifierKeys.Control | ModifierKeys.Alt), "Ctrl + Alt", "Ctrl+Alt preview");
        AssertEqual(KeyMacroHelper.FormatShortcutPreview(Key.D1, ModifierKeys.Control | ModifierKeys.Alt), "Ctrl + Alt + 1", "Ctrl+Alt+1 preview");
        AssertEqual(KeyMacroHelper.FormatShortcutPreview(Key.None, ModifierKeys.Control), "Ctrl", "Ctrl preview");
        AssertEqual(KeyMacroHelper.FormatShortcutPreview(Key.None, ModifierKeys.None), "Press keys...", "None preview");

        Console.WriteLine("[PASS] KeyMacroHelper.FormatShortcutPreview tests");

        // 3. Test MainViewModel Key Recording
        var deviceService = new MockDeviceService();
        var windowService = new WindowsWindowService();
        var graphicsAdapter = new SkiaGraphicsAdapter();
        var layerProvider = new YamlLayerProvider("config.yaml", windowService, graphicsAdapter, deviceService);
        var layerManager = new LayerManager(deviceService, graphicsAdapter, windowService);
        var vm = new MainViewModel(layerProvider, graphicsAdapter, deviceService, layerManager, "config.yaml");

        AssertEqual(vm.Layers.Count > 0, true, "Layers loaded");
        vm.SelectedLayer = vm.Layers[0];
        vm.SelectedLayer.SelectedButton = vm.SelectedLayer.Buttons[0];
        AssertEqual(vm.SelectedButton != null, true, "SelectedButton is not null");

        // Start recording
        vm.IsRecordingKeystrokes = true;
        AssertEqual(vm.IsRecordingKeystrokes, true, "Recording started");

        // Simulate preview of pressing Ctrl, then Alt (chord)
        vm.UpdateKeyPreview(Key.None, ModifierKeys.Control);
        AssertEqual(vm.RecordedShortcutPreview, "Ctrl", "Preview Ctrl");

        vm.UpdateKeyPreview(Key.None, ModifierKeys.Control | ModifierKeys.Alt);
        AssertEqual(vm.RecordedShortcutPreview, "Ctrl + Alt", "Preview Ctrl+Alt");

        // Simulate recording Ctrl+Alt combination
        vm.RecordKeystroke(Key.None, ModifierKeys.Control | ModifierKeys.Alt);
        AssertEqual(vm.SelectedButton!.ActionValue, "{{ctrl+alt}}", "ActionValue recorded as {{ctrl+alt}}");
        AssertEqual(vm.RecordedShortcutPreview, "Ctrl + Alt", "Recorded preview Ctrl+Alt");

        // Simulate recording Ctrl+Alt+1 with replacement
        vm.AppendRecordedKeystrokes = false;
        vm.RecordKeystroke(Key.D1, ModifierKeys.Control | ModifierKeys.Alt);
        AssertEqual(vm.SelectedButton.ActionValue, "{{ctrl+alt+1}}", "ActionValue recorded as {{ctrl+alt+1}}");
        AssertEqual(vm.RecordedShortcutPreview, "Ctrl + Alt + 1", "Recorded preview Ctrl+Alt+1");

        // Simulate appending keystrokes
        vm.AppendRecordedKeystrokes = true;
        vm.RecordKeystroke(Key.None, ModifierKeys.Control | ModifierKeys.Shift);
        AssertEqual(vm.SelectedButton.ActionValue, "{{ctrl+alt+1}}{{ctrl+shift}}", "Appended ActionValue");

        // Test Backspace
        vm.BackspaceMacro();
        AssertEqual(vm.SelectedButton.ActionValue, "{{ctrl+alt+1}}", "Backspace removed appended macro");

        // Test Clear
        vm.ClearMacro();
        AssertEqual(vm.SelectedButton.ActionValue, string.Empty, "Clear emptied macro");

        // Test Undo
        AssertEqual(vm.CanUndo, true, "CanUndo is true");
        vm.UndoCommand.Execute(null);
        AssertEqual(vm.SelectedButton.ActionValue, "{{ctrl+alt+1}}", "Undo restored {{ctrl+alt+1}}");

        Console.WriteLine("[PASS] MainViewModel recording and history tests");

        // 4. Test MacroAction parsing
        var macroAction = new MacroAction("{{ctrl+alt}}");
        var macroAction2 = new MacroAction("{{ctrl+alt+1}}");

        Console.WriteLine("[PASS] MacroAction instantiation tests");
        Console.WriteLine("=== ALL TESTS PASSED SUCCESSFULLY ===");
    }

    private static void AssertEqual<T>(T actual, T expected, string testName)
    {
        if (!object.Equals(actual, expected))
        {
            throw new Exception($"Test '{testName}' FAILED: expected '{expected}', got '{actual}'");
        }
    }
}
