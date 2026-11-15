using System.Diagnostics;
using System.Text;
using Warpdeck.Core.Domain;

namespace Warpdeck.Infrastructure.Adapters.Windows;

public class WindowsWindowService : IWindowService
{
    public WindowInfo? GetForegroundWindowInfo()
    {
        IntPtr hwnd = NativeMethods.GetForegroundWindow();
        if (hwnd == IntPtr.Zero) return null;

        string processName = "Unknown";
        NativeMethods.GetWindowThreadProcessId(hwnd, out uint pid);
        try
        {
            using var process = Process.GetProcessById((int)pid);
            processName = process.ProcessName;
        }
        catch
        {
            // Ignore
        }

        int length = NativeMethods.GetWindowTextLength(hwnd);
        StringBuilder sb = new StringBuilder(length + 1);
        NativeMethods.GetWindowText(hwnd, sb, sb.Capacity);
        string title = sb.ToString();

        return new WindowInfo(processName, title);
    }
}
