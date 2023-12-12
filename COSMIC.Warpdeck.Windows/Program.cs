using System;
using System.Windows.Forms;
using Autofac;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.Monitor;
using COSMIC.Warpdeck.Windows.Adapter;
using COSMIC.Warpdeck.Windows.Forms;

namespace COSMIC.Warpdeck.Windows
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadExit += (_, _) =>
            {
                WindowsWarpdeckApp.Container.Resolve<MonitorManager>().StopListening();
                WindowsWarpdeckApp.Container.Resolve<DeviceManager>().ClearDevices();
            };
            Application.Run(new MainForm(args));
        }
    }
}