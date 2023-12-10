#nullable enable
using System;
using System.Diagnostics;
using System.Windows.Forms;


namespace COSMIC.Warpdeck.Windows
{
    public partial class MainForm : Form
    {
        private readonly WarpdeckApp _app;
        private readonly WarpDeckWindowsApp _windowsApp;

        public MainForm(string[] args)
        {
            InitializeComponent();
            _windowsApp = new WarpDeckWindowsApp(args);
            _app = new();
            WarpdeckApp.Container = WarpDeckWindowsApp.Container;
            _windowsApp.RegisterDependencies();
            _windowsApp.StartPresentation();
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

        private void NotifyIcon_Menu_OpenClipboard_OnClick(object? sender, EventArgs e)
        {
            ClipboardForm clipboardForm = new();
            clipboardForm.Show();
        }
    }
}