using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Warpdeck.Core.Domain;

namespace Warpdeck.Infrastructure.Adapters.Windows;

public class MacroAction : IAction
{
    private readonly string _macro;
    private static readonly Regex MacroRegex = new(@"\{\{(.*?)\}\}|([^{]+)", RegexOptions.Compiled);

    public MacroAction(string macro)
    {
        _macro = macro;
    }

    public void Execute()
    {
        var inputs = ParseMacro();
        SendInputs(inputs);
    }

    private List<NativeMethods.INPUT> ParseMacro()
    {
        var inputs = new List<NativeMethods.INPUT>();
        var matches = MacroRegex.Matches(_macro);

        foreach (Match match in matches)
        {
            if (match.Groups[1].Success)
            {
                inputs.AddRange(ProcessKeyBlock(match.Groups[1].Value));
            }
            else if (match.Groups[2].Success)
            {
                inputs.AddRange(ProcessTextBlock(match.Groups[2].Value));
            }
        }

        return inputs;
    }

    private void SendInputs(List<NativeMethods.INPUT> inputs)
    {
        if (inputs.Count == 0) return;

        foreach (var input in inputs)
        {
            NativeMethods.SendInput(1, [input], Marshal.SizeOf(typeof(NativeMethods.INPUT)));
            Thread.Sleep(10); // Small delay between inputs for reliability
        }
    }

    private List<NativeMethods.INPUT> ProcessTextBlock(string text)
    {
        var inputs = new List<NativeMethods.INPUT>();
        foreach (char c in text)
        {
            inputs.Add(CreateUnicodeInput(c, isKeyUp: false));
            inputs.Add(CreateUnicodeInput(c, isKeyUp: true));
        }
        return inputs;
    }

    private List<NativeMethods.INPUT> ProcessKeyBlock(string block)
    {
        var inputs = new List<NativeMethods.INPUT>();
        var modifiers = new List<ushort>();
        var parts = block.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var part in parts)
        {
            ushort vk = MapToVk(part);
            if (IsModifier(part))
            {
                modifiers.Add(vk);
                inputs.Add(CreateKeyInput(vk, isKeyUp: false));
            }
            else
            {
                inputs.Add(CreateKeyInput(vk, isKeyUp: false));
                inputs.Add(CreateKeyInput(vk, isKeyUp: true));
            }
        }

        AddModifierReleases(inputs, modifiers);
        return inputs;
    }

    private void AddModifierReleases(List<NativeMethods.INPUT> inputs, List<ushort> modifiers)
    {
        for (int i = modifiers.Count - 1; i >= 0; i--)
        {
            inputs.Add(CreateKeyInput(modifiers[i], isKeyUp: true));
        }
    }

    private NativeMethods.INPUT CreateUnicodeInput(char c, bool isKeyUp)
    {
        return new NativeMethods.INPUT
        {
            type = NativeMethods.INPUT_KEYBOARD,
            u = new NativeMethods.InputUnion
            {
                ki = new NativeMethods.KEYBDINPUT
                {
                    wVk = 0,
                    wScan = (ushort)c,
                    dwFlags = NativeMethods.KEYEVENTF_UNICODE | (isKeyUp ? NativeMethods.KEYEVENTF_KEYUP : 0)
                }
            }
        };
    }

    private NativeMethods.INPUT CreateKeyInput(ushort vk, bool isKeyUp)
    {
        var flags = GetKeyFlags(vk, isKeyUp);
        var scanCode = (ushort)NativeMethods.MapVirtualKey(vk, NativeMethods.MAPVK_VK_TO_VSC);

        return new NativeMethods.INPUT
        {
            type = NativeMethods.INPUT_KEYBOARD,
            u = new NativeMethods.InputUnion
            {
                ki = new NativeMethods.KEYBDINPUT
                {
                    wVk = vk,
                    wScan = scanCode,
                    dwFlags = flags,
                    time = 0,
                    dwExtraInfo = IntPtr.Zero
                }
            }
        };
    }

    private uint GetKeyFlags(ushort vk, bool isKeyUp)
    {
        uint flags = isKeyUp ? NativeMethods.KEYEVENTF_KEYUP : 0;
        if (IsExtendedKey(vk))
        {
            flags |= NativeMethods.KEYEVENTF_EXTENDEDKEY;
        }
        return flags;
    }

    private bool IsModifier(string key)
    {
        return key.ToLower() switch
        {
            "ctrl" or "control" or "alt" or "menu" or "shift" or "win" or "super" or "command" => true,
            _ => false
        };
    }

    private bool IsExtendedKey(ushort vk)
    {
        return vk switch
        {
            0x21 or 0x22 or 0x23 or 0x24 or 0x25 or 0x26 or 0x27 or 0x28 or 0x2D or 0x2E => true, // Arrows, Ins, Del, Home, End, PgUp, PgDn
            0x5B or 0x5C => true, // LWin, RWin
            0x6F => true, // Numpad divide
            0xA1 or 0xA3 or 0xA5 => true, // RShift, RControl, RMenu
            >= 0xAD and <= 0xB3 => true, // Media keys (mute, voldown, volup, next, prev, stop, playpause)
            _ => false
        };
    }

    private ushort MapToVk(string key)
    {
        var normalizedKey = key.ToLower();

        return normalizedKey switch
        {
            "ctrl" or "control" => 0x11, // VK_CONTROL
            "alt" or "menu" => 0x12, // VK_MENU
            "shift" => 0x10, // VK_SHIFT
            "win" or "super" or "command" => 0x5B, // VK_LWIN
            _ => MapSpecialKeyToVk(normalizedKey)
        };
    }

    private ushort MapSpecialKeyToVk(string key)
    {
        return key switch
        {
            "tab" => 0x09,
            "enter" or "return" => 0x0D,
            "esc" or "escape" => 0x1B,
            "space" => 0x20,
            "backspace" or "back" => 0x08,
            "delete" or "del" => 0x2E,
            "insert" or "ins" => 0x2D,
            "home" => 0x24,
            "end" => 0x23,
            "pageup" or "pgup" => 0x21,
            "pagedown" or "pgdn" => 0x22,
            "up" => 0x26,
            "down" => 0x28,
            "left" => 0x25,
            "right" => 0x27,
            "capslock" or "caps" => 0x14,
            "numlock" => 0x90,
            "scrolllock" => 0x91,
            "prtscn" or "printscreen" or "snapshot" => 0x2C,
            "pause" or "break" => 0x13,
            "f1" => 0x70,
            "f2" => 0x71,
            "f3" => 0x72,
            "f4" => 0x73,
            "f5" => 0x74,
            "f6" => 0x75,
            "f7" => 0x76,
            "f8" => 0x77,
            "f9" => 0x78,
            "f10" => 0x79,
            "f11" => 0x7A,
            "f12" => 0x7B,
            "f13" => 0x7C,
            "f14" => 0x7D,
            "f15" => 0x7E,
            "f16" => 0x7F,
            "f17" => 0x80,
            "f18" => 0x81,
            "f19" => 0x82,
            "f20" => 0x83,
            "f21" => 0x84,
            "f22" => 0x85,
            "f23" => 0x86,
            "f24" => 0x87,
            "numpad0" or "num0" => 0x60,
            "numpad1" or "num1" => 0x61,
            "numpad2" or "num2" => 0x62,
            "numpad3" or "num3" => 0x63,
            "numpad4" or "num4" => 0x64,
            "numpad5" or "num5" => 0x65,
            "numpad6" or "num6" => 0x66,
            "numpad7" or "num7" => 0x67,
            "numpad8" or "num8" => 0x68,
            "numpad9" or "num9" => 0x69,
            "multiply" or "num*" => 0x6A,
            "add" or "num+" => 0x6B,
            "separator" => 0x6C,
            "subtract" or "num-" => 0x6D,
            "decimal" or "num." => 0x6E,
            "divide" or "num/" => 0x6F,
            "mute" or "volumemute" => 0xAD,
            "voldown" or "volumedown" => 0xAE,
            "volup" or "volumeup" => 0xAF,
            "next" or "medianext" => 0xB0,
            "prev" or "mediaprev" => 0xB1,
            "stop" or "mediastop" => 0xB2,
            "playpause" or "mediaplaypause" => 0xB3,
            ";" or "semicolon" => 0xBA,
            "=" or "plus" or "equal" => 0xBB,
            "," or "comma" => 0xBC,
            "-" or "minus" => 0xBD,
            "." or "period" => 0xBE,
            "/" or "slash" => 0xBF,
            "`" or "~" or "tilde" or "grave" => 0xC0,
            "[" or "openbracket" => 0xDB,
            "\\" or "backslash" => 0xDC,
            "]" or "closebracket" => 0xDD,
            "'" or "quote" => 0xDE,
            _ => (ushort)(key.Length == 1 ? char.ToUpper(key[0]) : 0)
        };
    }
}
