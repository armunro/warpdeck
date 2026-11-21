using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Warpdeck.UI.Common;
using Warpdeck.UI.ViewModels;

namespace Warpdeck.UI.Views;

public partial class MainWindow : Window
{
    private Point? _dragStartPoint;
    private ButtonViewModel? _draggedButton;
    private readonly LowLevelKeyboardHook _keyboardHook = new();
    private MainViewModel? _viewModel;

    public MainWindow()
    {
        InitializeComponent();

        _keyboardHook.KeyRecorded += (key, modifiers) =>
        {
            Dispatcher.Invoke(() =>
            {
                if (_viewModel is { IsRecordingKeystrokes: true })
                {
                    _viewModel.RecordKeystroke(key, modifiers);
                }
            });
        };

        _keyboardHook.KeyPreviewChanged += (key, modifiers) =>
        {
            Dispatcher.Invoke(() =>
            {
                if (_viewModel is { IsRecordingKeystrokes: true })
                {
                    _viewModel.UpdateKeyPreview(key, modifiers);
                }
            });
        };

        _keyboardHook.RecordingStopped += () =>
        {
            Dispatcher.Invoke(() =>
            {
                if (_viewModel != null)
                {
                    _viewModel.IsRecordingKeystrokes = false;
                }
            });
        };

        DataContextChanged += MainWindow_DataContextChanged;
        Deactivated += MainWindow_Deactivated;
        Closed += MainWindow_Closed;
    }

    private void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        if (e.NewValue is MainViewModel vm)
        {
            _viewModel = vm;
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }
        else
        {
            _viewModel = null;
            _keyboardHook.Stop();
        }
    }

    private readonly HashSet<Key> _previewPressedKeys = new();
    private ModifierKeys _previewChordModifiers = ModifierKeys.None;
    private bool _previewNonModifierPressed;

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.IsRecordingKeystrokes))
        {
            _previewPressedKeys.Clear();
            _previewChordModifiers = ModifierKeys.None;
            _previewNonModifierPressed = false;

            if (_viewModel?.IsRecordingKeystrokes == true)
            {
                _keyboardHook.Start();
            }
            else
            {
                _keyboardHook.Stop();
            }
        }
    }

    private void MainWindow_Deactivated(object? sender, EventArgs e)
    {
        // Stop recording when window loses focus to avoid intercepting external keystrokes
        if (_viewModel?.IsRecordingKeystrokes == true)
        {
            _viewModel.IsRecordingKeystrokes = false;
        }
        _keyboardHook.Stop();
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        _keyboardHook.Dispose();
    }

    private void ColorPreset_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string colorHex && DataContext is MainViewModel vm && vm.SelectedButton != null)
        {
            var normalized = ColorHelper.NormalizeHex(colorHex);
            vm.SelectedButton.Color = normalized;
            ColorHelper.AddRecentColor(normalized);
            vm.RefreshCurrentButtonPreview();
        }
    }

    private void PickLayerColor_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm && vm.SelectedLayer != null)
        {
            var currentHex = vm.SelectedLayer.Color ?? "#000000";
            if (ColorPickerDialog.ShowDialog(this, currentHex, out var selectedHex))
            {
                vm.SelectedLayer.Color = selectedHex;
                vm.RefreshCurrentLayerPreviews();
            }
        }
    }

    private void PickButtonColor_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm && vm.SelectedButton != null)
        {
            var currentHex = vm.SelectedButton.Color ?? "#0070F3";
            if (ColorPickerDialog.ShowDialog(this, currentHex, out var selectedHex))
            {
                vm.SelectedButton.Color = selectedHex;
                vm.RefreshCurrentButtonPreview();
            }
        }
    }

    private void PickButtonFontColor_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm && vm.SelectedButton != null)
        {
            var currentHex = vm.SelectedButton.FontColor ?? "#FFFFFF";
            if (ColorPickerDialog.ShowDialog(this, currentHex, out var selectedHex))
            {
                vm.SelectedButton.FontColor = selectedHex;
                vm.RefreshCurrentButtonPreview();
            }
        }
    }

    private void MatrixButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _dragStartPoint = e.GetPosition(this);
        if (sender is FrameworkElement element && element.DataContext is ButtonViewModel btnVm)
        {
            _draggedButton = btnVm;
        }
        else
        {
            _draggedButton = null;
        }
    }

    private void MatrixButton_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed || _dragStartPoint == null || _draggedButton == null)
            return;

        var currentPoint = e.GetPosition(this);
        var diff = _dragStartPoint.Value - currentPoint;

        if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
            Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
        {
            var sourceButton = _draggedButton;
            _dragStartPoint = null;
            _draggedButton = null;

            var dragData = new DataObject(typeof(ButtonViewModel), sourceButton);
            var effects = DragDropEffects.Move | DragDropEffects.Copy;
            try
            {
                DragDrop.DoDragDrop((DependencyObject)sender, dragData, effects);
            }
            finally
            {
                // Ensure all drop indicators are cleared after drag operation ends
                if (DataContext is MainViewModel mainVm && mainVm.SelectedLayer != null)
                {
                    foreach (var button in mainVm.SelectedLayer.Buttons)
                    {
                        button.IsDropTarget = false;
                    }
                }
            }
        }
    }

    private void MatrixButton_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(typeof(ButtonViewModel)))
        {
            var isShiftDown = (e.KeyStates & DragDropKeyStates.ShiftKey) != 0 ||
                              (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;

            e.Effects = isShiftDown ? DragDropEffects.Copy : DragDropEffects.Move;

            if (sender is FrameworkElement element && element.DataContext is ButtonViewModel btnVm)
            {
                btnVm.IsDropTarget = true;
            }

            e.Handled = true;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
    }

    private void MatrixButton_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(typeof(ButtonViewModel)))
        {
            var isShiftDown = (e.KeyStates & DragDropKeyStates.ShiftKey) != 0 ||
                              (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;

            e.Effects = isShiftDown ? DragDropEffects.Copy : DragDropEffects.Move;

            if (sender is FrameworkElement element && element.DataContext is ButtonViewModel btnVm)
            {
                if (!btnVm.IsDropTarget)
                {
                    btnVm.IsDropTarget = true;
                }
            }

            e.Handled = true;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
    }

    private void MatrixButton_DragLeave(object sender, DragEventArgs e)
    {
        if (sender is FrameworkElement element && element.DataContext is ButtonViewModel btnVm)
        {
            var pos = e.GetPosition(element);
            if (pos.X < 0 || pos.Y < 0 || pos.X >= element.ActualWidth || pos.Y >= element.ActualHeight)
            {
                btnVm.IsDropTarget = false;
            }
        }
    }

    private void MatrixButton_Drop(object sender, DragEventArgs e)
    {
        if (sender is FrameworkElement element && element.DataContext is ButtonViewModel targetButton)
        {
            targetButton.IsDropTarget = false;

            if (e.Data.GetData(typeof(ButtonViewModel)) is ButtonViewModel sourceButton)
            {
                var isShiftDown = (e.KeyStates & DragDropKeyStates.ShiftKey) != 0 ||
                                  (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;

                if (DataContext is MainViewModel mainVm)
                {
                    mainVm.MoveOrSwapButton(sourceButton, targetButton, isShiftDown);
                }
            }
        }
        e.Handled = true;
    }

    private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (_keyboardHook.IsHooked)
        {
            e.Handled = true;
            return;
        }

        if (DataContext is MainViewModel vm)
        {
            if (vm.IsRecordingKeystrokes)
            {
                var key = e.Key == Key.System ? e.SystemKey : e.Key;
                if (key == Key.ImeProcessed)
                {
                    key = e.ImeProcessedKey;
                }

                _previewPressedKeys.Add(key);
                var currentModifiers = ComputeWpfModifiers();

                if (KeyMacroHelper.IsModifierKey(key))
                {
                    _previewChordModifiers |= currentModifiers;
                    vm.UpdateKeyPreview(Key.None, _previewChordModifiers);
                    e.Handled = true;
                    return;
                }

                if (key == Key.Escape && currentModifiers == ModifierKeys.None)
                {
                    vm.IsRecordingKeystrokes = false;
                    e.Handled = true;
                    return;
                }

                _previewNonModifierPressed = true;
                var effectiveModifiers = _previewChordModifiers | currentModifiers;
                vm.RecordKeystroke(key, effectiveModifiers);
                e.Handled = true;
                return;
            }

            // If focus is in a TextBox or editable control, let standard text editing handle Delete/Undo
            if (e.OriginalSource is System.Windows.Controls.Primitives.TextBoxBase ||
                e.OriginalSource is PasswordBox ||
                Keyboard.FocusedElement is System.Windows.Controls.Primitives.TextBoxBase ||
                Keyboard.FocusedElement is PasswordBox)
            {
                return;
            }

            // Undo: Ctrl+Z
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Z)
            {
                if (vm.CanUndo)
                {
                    vm.UndoCommand.Execute(null);
                    e.Handled = true;
                    return;
                }
            }
            // Redo: Ctrl+Y or Ctrl+Shift+Z
            else if ((Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift) && e.Key == Key.Z) ||
                     (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Y))
            {
                if (vm.CanRedo)
                {
                    vm.RedoCommand.Execute(null);
                    e.Handled = true;
                    return;
                }
            }
            // Delete Key: Delete selected button
            else if (e.Key == Key.Delete && Keyboard.Modifiers == ModifierKeys.None)
            {
                if (vm.SelectedButton != null && vm.DeleteButtonCommand.CanExecute(null))
                {
                    vm.DeleteButtonCommand.Execute(null);
                    e.Handled = true;
                    return;
                }
            }
        }
    }

    private void MainWindow_PreviewKeyUp(object sender, KeyEventArgs e)
    {
        if (_keyboardHook.IsHooked)
        {
            e.Handled = true;
            return;
        }

        if (DataContext is MainViewModel vm && vm.IsRecordingKeystrokes)
        {
            var key = e.Key == Key.System ? e.SystemKey : e.Key;
            _previewPressedKeys.Remove(key);
            var currentModifiers = ComputeWpfModifiers();

            if (KeyMacroHelper.IsModifierKey(key))
            {
                if (!_previewNonModifierPressed && _previewChordModifiers != ModifierKeys.None)
                {
                    vm.RecordKeystroke(Key.None, _previewChordModifiers);
                    _previewNonModifierPressed = true;
                }

                if (currentModifiers == ModifierKeys.None)
                {
                    _previewChordModifiers = ModifierKeys.None;
                    _previewNonModifierPressed = false;
                    vm.UpdateKeyPreview(Key.None, ModifierKeys.None);
                }
                else
                {
                    _previewChordModifiers = currentModifiers;
                    vm.UpdateKeyPreview(Key.None, _previewChordModifiers);
                }
            }
            else
            {
                if (_previewPressedKeys.Count == 0)
                {
                    _previewChordModifiers = ModifierKeys.None;
                    _previewNonModifierPressed = false;
                    vm.UpdateKeyPreview(Key.None, ModifierKeys.None);
                }
            }

            e.Handled = true;
        }
    }

    private ModifierKeys ComputeWpfModifiers()
    {
        var modifiers = Keyboard.Modifiers;
        if (_previewPressedKeys.Contains(Key.LWin) || _previewPressedKeys.Contains(Key.RWin))
        {
            modifiers |= ModifierKeys.Windows;
        }
        if (_previewPressedKeys.Contains(Key.LeftCtrl) || _previewPressedKeys.Contains(Key.RightCtrl))
        {
            modifiers |= ModifierKeys.Control;
        }
        if (_previewPressedKeys.Contains(Key.LeftAlt) || _previewPressedKeys.Contains(Key.RightAlt))
        {
            modifiers |= ModifierKeys.Alt;
        }
        if (_previewPressedKeys.Contains(Key.LeftShift) || _previewPressedKeys.Contains(Key.RightShift))
        {
            modifiers |= ModifierKeys.Shift;
        }
        return modifiers;
    }
}
