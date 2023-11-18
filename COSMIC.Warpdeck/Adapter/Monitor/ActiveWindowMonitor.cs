using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using COSMIC.Warpdeck.Domain.Monitor;

namespace COSMIC.Warpdeck.Adapter.Monitor
{
    public class ActiveWindowMonitor : IMonitor
    {
        public event MonitorChangeEventDelegate OnMonitorChange;
        private WinEventDelegate _winEventDelegate = null;

        #region Windows user32.dll Externs

        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        #endregion

        public ActiveWindowMonitor()
        {
            _winEventDelegate = WinEventProc;
            SetWinEventHook(EVENT_SYSTEM_FOREGROUND,
                EVENT_SYSTEM_FOREGROUND,
                IntPtr.Zero, _winEventDelegate, 0, 0,
                WINEVENT_OUTOFCONTEXT);
        }

        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            IntPtr handle = IntPtr.Zero;
            StringBuilder Buff = new StringBuilder(nChars);
            handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }

            return null;
        }

        private uint GetActiveWindowPid()
        {
            IntPtr handle = IntPtr.Zero;
            handle = GetForegroundWindow();
            uint activeWindowPid;
            GetWindowThreadProcessId(handle, out activeWindowPid);

            return activeWindowPid;
        }

        private string GetActiveWindowExePath(uint pid)
        {
            try
            {
                var process = Process.GetProcessById(Convert.ToInt32(pid));
                return process.MainModule.FileName;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return "";
        }

        public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild,
            uint dwEventThread, uint dwmsEventTime)
        {
            uint pid = GetActiveWindowPid();
            OnMonitorChange?.Invoke(this, new MonitorChangeEventArgs()
            {
                EventData = new Dictionary<string, string>()
                {
                    { "WindowTitle", GetActiveWindowTitle() },
                    { "WindowPid", pid.ToString() },
                    { "WindowAppPath", GetActiveWindowExePath(pid) }
                }
            });
        }
    }
}