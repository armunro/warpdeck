using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Warpdeck.UI.Common;

public sealed class LowLevelKeyboardHook : IDisposable
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private const int WM_SYSKEYDOWN = 0x0104;
    private const int WM_SYSKEYUP = 0x0105;

    private const int VK_ESCAPE = 0x1B;
    private const int VK_SHIFT = 0x10;
    private const int VK_CONTROL = 0x11;
    private const int VK_MENU = 0x12; // Alt
    private const int VK_LWIN = 0x5B;
    private const int VK_RWIN = 0x5C;
    private const int VK_LSHIFT = 0xA0;
    private const int VK_RSHIFT = 0xA1;
    private const int VK_LCONTROL = 0xA2;
    private const int VK_RCONTROL = 0xA3;
    private const int VK_LMENU = 0xA4;
    private const int VK_RMENU = 0xA5;

    public delegate void KeyRecordedHandler(Key key, ModifierKeys modifiers);
    public delegate void KeyPreviewHandler(Key key, ModifierKeys modifiers);
    public delegate void RecordingStoppedHandler();

    public event KeyRecordedHandler? KeyRecorded;
    public event KeyPreviewHandler? KeyPreviewChanged;
    public event RecordingStoppedHandler? RecordingStopped;

    private readonly LowLevelKeyboardProc _proc;
    private IntPtr _hookId = IntPtr.Zero;
    private bool _isDisposed;

    // Internal state tracking for pressed keys and active modifier chord
    private readonly HashSet<int> _pressedKeys = new();
    private ModifierKeys _chordModifiers = ModifierKeys.None;
    private bool _nonModifierPressedInChord;

    public bool IsHooked => _hookId != IntPtr.Zero;

    public LowLevelKeyboardHook()
    {
        // Maintain a reference to delegate to prevent garbage collection
        _proc = HookCallback;
    }

    public void Start()
    {
        if (_hookId != IntPtr.Zero) return;

        ResetState();

        IntPtr hModule = GetModuleHandle(null);
        if (hModule == IntPtr.Zero)
        {
            using var curProcess = Process.GetCurrentProcess();
            using var curModule = curProcess.MainModule;
            hModule = curModule != null ? GetModuleHandle(curModule.ModuleName) : IntPtr.Zero;
        }

        _hookId = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, hModule, 0);
    }

    public void Stop()
    {
        if (_hookId != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_hookId);
            _hookId = IntPtr.Zero;
        }
        ResetState();
    }

    private void ResetState()
    {
        _pressedKeys.Clear();
        _chordModifiers = ModifierKeys.None;
        _nonModifierPressedInChord = false;
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            int msg = wParam.ToInt32();
            var kbStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
            int vkCode = (int)kbStruct.vkCode;

            bool isKeyDown = msg == WM_KEYDOWN || msg == WM_SYSKEYDOWN;
            bool isKeyUp = msg == WM_KEYUP || msg == WM_SYSKEYUP;

            if (isKeyDown || isKeyUp)
            {
                bool isModKey = IsModifierVk(vkCode);

                if (isKeyDown)
                {
                    _pressedKeys.Add(vkCode);
                    var currentModifiers = ComputeCurrentModifiers();

                    if (isModKey)
                    {
                        _chordModifiers |= currentModifiers;
                        KeyPreviewChanged?.Invoke(Key.None, _chordModifiers);
                        return (IntPtr)1; // Suppress modifier-only key events (e.g. Win key opening Start menu or Alt menu)
                    }

                    if (vkCode == VK_ESCAPE && currentModifiers == ModifierKeys.None)
                    {
                        RecordingStopped?.Invoke();
                        return (IntPtr)1;
                    }

                    _nonModifierPressedInChord = true;
                    var effectiveModifiers = _chordModifiers | currentModifiers;
                    var key = KeyInterop.KeyFromVirtualKey(vkCode);

                    KeyRecorded?.Invoke(key, effectiveModifiers);
                    return (IntPtr)1; // Suppress keystroke so OS hotkeys (e.g. Win+Shift+Right, Alt+Tab) do not trigger
                }
                else if (isKeyUp)
                {
                    _pressedKeys.Remove(vkCode);
                    var currentModifiers = ComputeCurrentModifiers();

                    if (isModKey)
                    {
                        // If user held modifier combination (e.g. Ctrl+Alt) and releases without typing another key,
                        // record the modifier combination itself
                        if (!_nonModifierPressedInChord && _chordModifiers != ModifierKeys.None)
                        {
                            KeyRecorded?.Invoke(Key.None, _chordModifiers);
                            _nonModifierPressedInChord = true; // Mark recorded so subsequent key releases in the same chord don't re-trigger
                        }

                        if (currentModifiers == ModifierKeys.None)
                        {
                            _chordModifiers = ModifierKeys.None;
                            _nonModifierPressedInChord = false;
                            KeyPreviewChanged?.Invoke(Key.None, ModifierKeys.None);
                        }
                        else
                        {
                            _chordModifiers = currentModifiers;
                            KeyPreviewChanged?.Invoke(Key.None, _chordModifiers);
                        }

                        return (IntPtr)1;
                    }
                    else
                    {
                        // Non-modifier released
                        if (_pressedKeys.Count == 0)
                        {
                            _chordModifiers = ModifierKeys.None;
                            _nonModifierPressedInChord = false;
                            KeyPreviewChanged?.Invoke(Key.None, ModifierKeys.None);
                        }

                        return (IntPtr)1;
                    }
                }
            }
        }

        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    private static bool IsModifierVk(int vkCode)
    {
        return vkCode is VK_LWIN or VK_RWIN or
               VK_SHIFT or VK_LSHIFT or VK_RSHIFT or
               VK_CONTROL or VK_LCONTROL or VK_RCONTROL or
               VK_MENU or VK_LMENU or VK_RMENU;
    }

    private ModifierKeys ComputeCurrentModifiers()
    {
        ModifierKeys modifiers = ModifierKeys.None;

        bool isWin = _pressedKeys.Contains(VK_LWIN) || _pressedKeys.Contains(VK_RWIN) ||
                     (GetAsyncKeyState(VK_LWIN) & 0x8000) != 0 || (GetAsyncKeyState(VK_RWIN) & 0x8000) != 0;

        bool isCtrl = _pressedKeys.Contains(VK_CONTROL) || _pressedKeys.Contains(VK_LCONTROL) || _pressedKeys.Contains(VK_RCONTROL) ||
                      (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0;

        bool isAlt = _pressedKeys.Contains(VK_MENU) || _pressedKeys.Contains(VK_LMENU) || _pressedKeys.Contains(VK_RMENU) ||
                     (GetAsyncKeyState(VK_MENU) & 0x8000) != 0;

        bool isShift = _pressedKeys.Contains(VK_SHIFT) || _pressedKeys.Contains(VK_LSHIFT) || _pressedKeys.Contains(VK_RSHIFT) ||
                       (GetAsyncKeyState(VK_SHIFT) & 0x8000) != 0;

        if (isWin) modifiers |= ModifierKeys.Windows;
        if (isCtrl) modifiers |= ModifierKeys.Control;
        if (isAlt) modifiers |= ModifierKeys.Alt;
        if (isShift) modifiers |= ModifierKeys.Shift;

        return modifiers;
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            Stop();
            _isDisposed = true;
        }
        GC.SuppressFinalize(this);
    }

    ~LowLevelKeyboardHook()
    {
        Dispose();
    }

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential)]
    private struct KBDLLHOOKSTRUCT
    {
        public uint vkCode;
        public uint scanCode;
        public uint flags;
        public uint time;
        public UIntPtr dwExtraInfo;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string? lpModuleName);

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);
}
