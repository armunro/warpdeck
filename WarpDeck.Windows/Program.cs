using System;
using System.Windows.Forms;
using Autofac;
using WarpDeck.Domain.Device;
using WarpDeck.Domain.Monitor;
using WarpDeck.UseCase.Device;

namespace WarpDeck.Windows
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
                WarpDeckWindowsApp.Container.Resolve<MonitorManager>().StopListening();
                WarpDeckWindowsApp.Container.Resolve<DeviceManager>().ClearDevices();
            };
            Application.Run(new MainForm(args));
        }
    }
}