using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Warpdeck.UI.Common;

public static class KeyMacroHelper
{
    public static bool IsModifierKey(Key key)
    {
        return key switch
        {
            Key.LeftCtrl or Key.RightCtrl or
            Key.LeftAlt or Key.RightAlt or
            Key.LeftShift or Key.RightShift or
            Key.LWin or Key.RWin or
            Key.System => true,
            _ => false
        };
    }

    public static string? ConvertToMacro(Key key, ModifierKeys modifiers)
    {
        if (IsModifierKey(key))
        {
            return null;
        }

        if (key == Key.None && modifiers == ModifierKeys.None)
        {
            return null;
        }

        var parts = new List<string>();

        if ((modifiers & ModifierKeys.Windows) != 0)
        {
            parts.Add("win");
        }
        if ((modifiers & ModifierKeys.Control) != 0)
        {
            parts.Add("ctrl");
        }
        if ((modifiers & ModifierKeys.Alt) != 0)
        {
            parts.Add("alt");
        }
        if ((modifiers & ModifierKeys.Shift) != 0)
        {
            parts.Add("shift");
        }

        if (key != Key.None)
        {
            var keyName = MapKeyToString(key);
            if (string.IsNullOrEmpty(keyName))
            {
                return null;
            }
            parts.Add(keyName);
        }

        if (parts.Count == 0)
        {
            return null;
        }

        // All recorded keystrokes are formatted as {{...}} macro blocks (e.g. {{ctrl+c}}, {{win+shift+s}}, {{ctrl+alt}}, {{f5}}, {{enter}})
        return $"{{{{{string.Join("+", parts)}}}}}";
    }

    public static string FormatShortcutPreview(Key key, ModifierKeys modifiers)
    {
        var parts = new List<string>();

        if ((modifiers & ModifierKeys.Windows) != 0) parts.Add("Win");
        if ((modifiers & ModifierKeys.Control) != 0) parts.Add("Ctrl");
        if ((modifiers & ModifierKeys.Alt) != 0) parts.Add("Alt");
        if ((modifiers & ModifierKeys.Shift) != 0) parts.Add("Shift");

        if (!IsModifierKey(key) && key != Key.None)
        {
            var keyName = MapKeyToString(key);
            if (!string.IsNullOrEmpty(keyName))
            {
                parts.Add(char.ToUpper(keyName[0]) + (keyName.Length > 1 ? keyName.Substring(1) : ""));
            }
        }

        return parts.Count > 0 ? string.Join(" + ", parts) : "Press keys...";
    }

    private static string? MapKeyToString(Key key)
    {
        return key switch
        {
            // Letters A-Z
            >= Key.A and <= Key.Z => key.ToString().ToLowerInvariant(),

            // Digits 0-9
            >= Key.D0 and <= Key.D9 => ((int)key - (int)Key.D0).ToString(),

            // Numpad 0-9 & math
            >= Key.NumPad0 and <= Key.NumPad9 => "numpad" + ((int)key - (int)Key.NumPad0),
            Key.Multiply => "multiply",
            Key.Add => "add",
            Key.Separator => "separator",
            Key.Subtract => "subtract",
            Key.Decimal => "decimal",
            Key.Divide => "divide",

            // Function keys F1-F24
            >= Key.F1 and <= Key.F24 => key.ToString().ToLowerInvariant(),

            // Navigation & editing
            Key.Up => "up",
            Key.Down => "down",
            Key.Left => "left",
            Key.Right => "right",
            Key.Home => "home",
            Key.End => "end",
            Key.PageUp or Key.Prior => "pageup",
            Key.PageDown or Key.Next => "pagedown",
            Key.Insert => "insert",
            Key.Delete => "delete",
            Key.Escape => "esc",
            Key.Enter or Key.Return => "enter",
            Key.Tab => "tab",
            Key.Space => "space",
            Key.Back => "backspace",

            // Lock & System keys
            Key.PrintScreen or Key.Snapshot => "prtscn",
            Key.Pause => "pause",
            Key.Capital => "capslock",
            Key.NumLock => "numlock",
            Key.Scroll => "scrolllock",

            // Media keys
            Key.VolumeUp => "volup",
            Key.VolumeDown => "voldown",
            Key.VolumeMute => "mute",
            Key.MediaNextTrack => "next",
            Key.MediaPreviousTrack => "prev",
            Key.MediaPlayPause => "playpause",
            Key.MediaStop => "stop",

            // OEM keys / Punctuation
            Key.OemMinus => "-",
            Key.OemPlus => "=",
            Key.OemOpenBrackets => "[",
            Key.OemCloseBrackets => "]",
            Key.OemQuotes => "'",
            Key.Oem1 or Key.OemSemicolon => ";",
            Key.OemQuestion => "/",
            Key.OemPeriod => ".",
            Key.OemComma => ",",
            Key.Oem3 or Key.OemTilde => "`",
            Key.Oem5 or Key.OemPipe => "\\",

            _ => null
        };
    }
}
