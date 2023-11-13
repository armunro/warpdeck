using System;
using System.Windows.Forms;
using Autofac;
using WarpDeck;
using WarpDeck.Application;
using WarpDeck.Application.Device;
using WarpDeck.Application.Monitor;
using WarpDeck.Domain;

namespace WarpDeckForms
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
                WarpDeckApp.Container.Resolve<MonitorManager>().StopListening();
                WarpDeckApp.Container.Resolve<DeviceManager>().ClearDevices();
            };
            Application.Run(new MainForm(args));
        }
    }
}