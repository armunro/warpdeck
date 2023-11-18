using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace COSMIC.Warpdeck.Windows.Plugins.Actions.Window.Hwnd
{
    
    public class WindowHandle
    {
        public WindowHandle(IntPtr hwnd)
        {
            Hwnd = hwnd;
        }

        public void Click()
        {
            HwndInterface.ClickHwnd(Hwnd);
        }

        // <summary>
        // Bring this window to the foreground
        // </summary>
        public bool Activate()
        {
            return HwndInterface.ActivateWindow(Hwnd);
        }

        // <summary>
        // Minimize this window
        // </summary>
        public bool Minimize()
        {
            return HwndInterface.MinimizeWindow(Hwnd);
        }

        // <summary>
        // Maximize this window
        // </summary>
        public bool Maximize()
        {
            return HwndInterface.MaximizeWindow(Hwnd);
        }

        public override bool Equals(object obj)
        {
            return !ReferenceEquals(null, obj) && (ReferenceEquals(this, obj) ||
                                                   obj.GetType() == typeof(WindowHandle) && Equals((WindowHandle)obj));
        }

        public bool Equals(WindowHandle obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return ReferenceEquals(this, obj) || obj.Hwnd.Equals(Hwnd);
        }

        public WindowHandle GetChild(string cls, string title)
        {
            return new WindowHandle(HwndInterface.GetHwndChild(Hwnd, cls, title));
        }

        public List<WindowHandle> GetChildren()
        {
            return HwndInterface.EnumChildren(Hwnd)
                .Select(ptr => new WindowHandle(ptr))
                .ToList();
        }

        public override int GetHashCode()
        {
            return Hwnd.GetHashCode();
        }

        public int GetMessageInt(WM msg)
        {
            return HwndInterface.GetMessageInt(Hwnd, msg);
        }

        public string GetMessageString(WM msg, uint param)
        {
            return HwndInterface.GetMessageString(Hwnd, msg, param);
        }

        public WindowHandle GetParent()
        {
            return new WindowHandle(HwndInterface.GetHwndParent(Hwnd));
        }


        public static WindowHandle GetWindowByTitle(string title)
        {
            return new WindowHandle(HwndInterface.GetHwndFromTitle(title));
        }

        public static WindowHandle GetActiveWindow()
        {
            return new WindowHandle(HwndInterface.GetActiveWindow());
        }

        public static WindowHandle GetWindowByClassName(string className)
        {
            return new WindowHandle(HwndInterface.GetHwndFromClass(className));
        }

        public static List<WindowHandle> GetWindows()
        {
            List<WindowHandle> list = new List<WindowHandle>();
            foreach (IntPtr ptr in HwndInterface.EnumHwnds())
            {
                list.Add(new WindowHandle(ptr));
            }

            return list;
        }

        public static bool operator ==(WindowHandle a, WindowHandle b)
        {
            if (ReferenceEquals(a, null))
            {
                return ReferenceEquals(b, null);
            }
            else if (ReferenceEquals(b, null))
            {
                return ReferenceEquals(a, null);
            }

            return (a.Hwnd == b.Hwnd);
        }

        public static bool operator !=(WindowHandle a, WindowHandle b)
        {
            return !(a == b);
        }

        public void SendMessage(WM msg, uint param1, string param2)
        {
            HwndInterface.SendMessage(Hwnd, msg, param1, param2);
        }

        public void SendMessage(WM msg, uint param1, uint param2)
        {
            HwndInterface.SendMessage(Hwnd, msg, param1, param2);
        }

        public override string ToString()
        {
            Point location = Location;
            Size size = Size;
            return string.Format("({0}) {1},{2}:{3}x{4} \"{5}\"",
                new object[] { Hwnd, location.X, location.Y, size.Width, size.Height, Title });
        }

        public string ClassName => HwndInterface.GetHwndClassName(Hwnd);

        public IntPtr Hwnd { get; private set; }

        public Point Location
        {
            get => HwndInterface.GetHwndPos(Hwnd);
            set => HwndInterface.SetHwndPos(Hwnd, value.X, value.Y);
        }

        public Size Size
        {
            get => HwndInterface.GetHwndSize(Hwnd);
            set => HwndInterface.SetHwndSize(Hwnd, value.Width, value.Height);
        }

        public string Text
        {
            get => HwndInterface.GetHwndText(Hwnd);
            set => HwndInterface.SetHwndText(Hwnd, value);
        }

        public string Title
        {
            get => HwndInterface.GetHwndTitle(Hwnd);
            set => HwndInterface.SetHwndTitle(Hwnd, value);
        }

        public uint Pid
        {
            get => HwndInterface.GetWindowPID(Hwnd);
        }
    }
}