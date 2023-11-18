using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using COSMIC.Warpdeck.Domain.Key;
using COSMIC.Warpdeck.Domain.Key.Action;
using COSMIC.Warpdeck.Domain.Key.Action.Descriptors;

namespace COSMIC.Warpdeck.Windows.Plugins.Actions.Launcher
{
    public class LauncherAction : KeyAction<LauncherActionModelModel>, IHasActionParameters
    {
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);


        [DllImport("user32.dll")]
        static extern IntPtr SetFocus(HandleRef hWnd);


        public LauncherAction(Dictionary<string, string> parameters) : base(parameters)
        {
        }

        public LauncherAction()
        {
        }

        public override void StartAction()
        {
            var matchingProcesses = Process.GetProcessesByName(Model.ProcessName);

            if (matchingProcesses.Any())
            {
                IntPtr handle = matchingProcesses.First().MainWindowHandle;
                SetForegroundWindow(handle);
                SetFocus(new HandleRef(null, handle)); // not needed
            }
            else
            {
                ProcessStartInfo process = new ProcessStartInfo(Model.AppPath, Model.Arguments);
                Process.Start(process); 
            }
        }


        public ActionParamDescriptorSet SpecifyParameters()
        {
            return new ActionParamDescriptorSet()
            {
                Parameters =
                {
                    new ActionParamDescriptor()
                    {
                        Name = "path",
                        Description = "The location of the application to open",
                        FriendlyName = "Path"
                    },
                    new ActionParamDescriptor()
                    {
                        Name = "processName",
                        Description = "The name of the process to look for when activating the window.",
                        FriendlyName = "Process Name"
                    },
                    new ActionParamDescriptor()
                    {
                        Name = "arguments",
                        Description = "Any commandline arguments to add",
                        FriendlyName = "Arguments"
                    }
                }
            };
        }
    }
}