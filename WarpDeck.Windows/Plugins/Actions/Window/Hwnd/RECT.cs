﻿using System.Runtime.InteropServices;

namespace WarpDeck.Windows.Plugins.Actions.Window.Hwnd
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}