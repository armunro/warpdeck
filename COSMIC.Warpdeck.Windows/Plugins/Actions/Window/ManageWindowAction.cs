#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using COSMIC.Warpdeck.Domain.Key;
using COSMIC.Warpdeck.Domain.Key.Action;
using COSMIC.Warpdeck.Domain.Key.Action.Descriptors;
using COSMIC.Warpdeck.Windows.Plugins.Actions.Window.Hwnd;

namespace COSMIC.Warpdeck.Windows.Plugins.Actions.Window
{
    public class ManageWindowAction : KeyAction<WindowManageModelModel>, IHasActionParameters
    {
        public ManageWindowAction(Dictionary<string, string> parameters) : base(parameters)
        {
        }

        public ManageWindowAction()
        {
        }

        public override void StartAction()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Model.ProcessName) || !string.IsNullOrWhiteSpace(Model.WindowTitle))
                {
                    ManageTargetedWindow();
                }
                else
                {
                    ManageWindow(WindowHandle.GetActiveWindow());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ManageTargetedWindow()
        {
            IEnumerable<Process> processes = Process.GetProcesses();
            if (!string.IsNullOrWhiteSpace(Model.ProcessName))
            {
                processes = processes.Where(x => x.ProcessName.ToLower().Contains(Model.ProcessName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(Model.WindowTitle))
            {
                processes = processes.Where(x =>
                    new WindowHandle(x.MainWindowHandle).Title.ToLower().Contains((string)Model.WindowTitle));
            }


            foreach (Process process in processes)
            {
                ManageWindow(new WindowHandle(process.MainWindowHandle));
            }
        }

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(HandleRef hWnd, int nCmdShow);
        
        
        public void ManageWindow(WindowHandle windowHandle)
        {
            if (Model.ScreenNumber >= 0)
            {
                var screen =  Screen.AllScreens[Model.ScreenNumber].WorkingArea;
                windowHandle.Location = new Point(screen.X - 8, screen.Y - 8);
                Console.WriteLine(screen);
            }

            switch (Model.WindowState)
            {
                case WindowState.Maximized:
                    windowHandle.Maximize();
                    break;
                case WindowState.Minimized:
                    windowHandle.Minimize();
                    break;
            }

            ShowWindowAsync(new HandleRef(null, windowHandle.Hwnd), 9);
            SetForegroundWindow(windowHandle.Hwnd);
            windowHandle.Activate();
            
            SetFocus(new HandleRef(null,windowHandle.Hwnd));

        }
        
        [DllImport("user32.dll")]
        static extern IntPtr SetFocus(HandleRef hWnd);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        
        public ActionParamDescriptorSet SpecifyParameters()
        {
            return new ActionParamDescriptorSet()
            {
                Parameters =
                {
                    new ActionParamDescriptor()
                    {
                        Name = "processName",
                        FriendlyName = "Process Name"
                    },
                    new ActionParamDescriptor()
                    {
                        Name = "windowTitle",
                        FriendlyName = "Window Title",
                    },
                    new ActionParamDescriptor()
                    {
                        Name = "screenNumber",
                        FriendlyName = "Screen Number",
                    },
                    new ActionParamDescriptor()
                    {
                        Name = "windowState",
                        FriendlyName = "Window State",
                    },
                    
                }
            };
        }
    }
}