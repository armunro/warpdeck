#nullable enable
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Autofac;
using COSMIC.Warpdeck.Domain.Clipboard;
using COSMIC.Warpdeck.Managers;
using COSMIC.Warpdeck.Windows.Adapter;
using COSMIC.Warpdeck.Windows.Adapter.Monitor;

namespace COSMIC.Warpdeck.Windows.Forms
{
    public partial class MainForm : Form
    {
        private readonly WarpdeckApp _app;
        private readonly WindowsWarpdeckApp _windowsWarpdeckApp;

        public MainForm(string[] args)
        {
            InitializeComponent();
            _app = new();
            WarpdeckApp.Container = WindowsWarpdeckApp.Container;
            _windowsWarpdeckApp = new WindowsWarpdeckApp(args);
            _windowsWarpdeckApp.RegisterDependencies();
            _windowsWarpdeckApp.StartPresentation();
            WindowsWarpdeckApp.Container.Resolve<IClipboardManager>().StartMonitoring();
            WarpdeckApp.Container.Resolve<DeviceManager>().AddMonitor(new ActiveWindowMonitor());
            
            _app.LoadDevices();
            Visible = false;
            Hide();
            Focus();
        }


        private void NotifyIcon_Menu_OpenUI_OnClick(object? sender, EventArgs e) =>
            Process.Start(new ProcessStartInfo("http://localhost:4300") { UseShellExecute = true });

        private void NotifyIcon_Menu_Reload_OnClick(object? sender, EventArgs e) =>
            _app.Reload();

        private void NotifyIcon_Menu_Save_OnClick(object? sender, EventArgs e) =>
            _app.Save();

        private void NotifyIcon_Menu_ExitI_OnClick(object? sender, EventArgs e)
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
        
    }
}