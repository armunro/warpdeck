#nullable enable
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Autofac;
using COSMIC.Warpdeck.Domain.Clipboard;
using COSMIC.Warpdeck.Managers;
using COSMIC.Warpdeck.Windows.Adapter.Monitor;

namespace COSMIC.Warpdeck.Windows
{
    public partial class MainForm : Form
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
            
            _warpdeckWindowsApp.LoadConfig();
            Visible = false;
            Hide();
        }


        private void NotifyIcon_Menu_OpenUI_OnClick(object? sender, EventArgs e) =>
            Process.Start(new ProcessStartInfo("http://localhost:4300") { UseShellExecute = true });

        private void NotifyIcon_Menu_Reload_OnClick(object? sender, EventArgs e) =>
            _warpdeckWindowsApp.ReloadConfig();

        private void NotifyIcon_Menu_Save_OnClick(object? sender, EventArgs e) =>
            _warpdeckWindowsApp.SaveConfig();

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