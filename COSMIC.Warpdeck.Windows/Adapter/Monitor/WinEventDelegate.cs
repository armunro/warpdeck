using System;

namespace COSMIC.Warpdeck.Windows.Adapter.Monitor
{
    delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild,
        uint dwEventThread, uint dwmsEventTime);
}