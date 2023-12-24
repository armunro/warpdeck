#nullable enable
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Autofac;
using COSMIC.Warpdeck.Domain.Clipboard;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.DeviceHost;
using COSMIC.Warpdeck.Managers;
using COSMIC.Warpdeck.Windows.Adapter;
using COSMIC.Warpdeck.Windows.Adapter.Monitor;
using Microsoft.Toolkit.Uwp.Notifications;

namespace COSMIC.Warpdeck.Windows
{
    public partial class MainForm : Form, IDeviceHostClient
    {
        private readonly WarpdeckWindowsApp _warpdeckWindowsApp;

        public MainForm(string[] args)
        {
            InitializeComponent();

            _warpdeckWindowsApp = new WarpdeckWindowsApp(args);
            _warpdeckWindowsApp.RegisterDependencies();
            _warpdeckWindowsApp.StartPresentation();
            _warpdeckWindowsApp.StartClipboardMonitor();
            _warpdeckWindowsApp.RegisterWindowsMonitors();
            WarpdeckAppContext.Container = WarpdeckWindowsApp.Container;     
            WarpdeckAppContext.Container.Resolve<DeviceHostManager>().RegisterHostClient(this);
            
            _warpdeckWindowsApp.LoadConfig();
            Visible = false;
            Hide();
            
            ShowStartedNotification();
        }

        private static void ShowStartedNotification()
        {
            new ToastContentBuilder()
                .AddText("Warpdeck is running in the background.")
                .AddText("Right click the system tray icon to get started.")
                .Show();  
        }

        private static void ShowReloadedNotification()
        {
            new ToastContentBuilder()
                .AddText("Configuration Reloaded")
                .AddText("Warpdeck configuration has been reloaded")
                .Show();
        }

        private static void ShowSavedNotification()
        {
            new ToastContentBuilder()
                .AddText("Configuration Saved")
                .AddText("Warpdeck configuration has been saved")
                .Show();
        }

        private void NotifyIcon_Menu_Reload_OnClick(object? sender, EventArgs e)
        {
            _warpdeckWindowsApp.ReloadConfig();
            ShowReloadedNotification();
        }

        private void NotifyIcon_Menu_Save_OnClick(object? sender, EventArgs e)
        {
            _warpdeckWindowsApp.SaveConfig();
            ShowSavedNotification();
        }

        private void NotifyIcon_Menu_Exit_OnClick(object? sender, EventArgs e)
        {
            NotifyIcon.Visible = false;
            Application.Exit();
        }

        private void OnFormClosing(object? sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Hide();
                ShowInTaskbar = false;
                e.Cancel = true;
            }
        }

        public DeviceHostHandle OpenDeviceHost(DeviceModel model)
        {
            DeviceHostHandle deviceHostHandle = null;
            this.Invoke((MethodInvoker) delegate
            {
                DeviceHost deviceHost = new DeviceHost(model);
                deviceHost.Show();
                deviceHostHandle = new WinFormsDeviceHostHandle()
                {
                    Device = model,
                    WinForm = deviceHost
                };
            });

            return deviceHostHandle;
        }

        private void NotifyIcon_Menu_OpenUI_OnClick(object? sender, EventArgs e) =>
            Process.Start(new ProcessStartInfo("http://localhost:4300") { UseShellExecute = true });
    }
}