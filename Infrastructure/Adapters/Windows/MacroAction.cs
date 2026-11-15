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
            0xA1 or 0xA3 or 0xA5 => true, // RShift, RControl, RMenu
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
            "backspace" => 0x08,
            "delete" => 0x2E,
            "up" => 0x26,
            "down" => 0x28,
            "left" => 0x25,
            "right" => 0x27,
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
            "s" => 0x53,
            "t" => 0x54,
            _ => (ushort)(key.Length == 1 ? char.ToUpper(key[0]) : 0)
        };
    }
}
