#nullable enable
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WarpDeck;
using WarpDeck.Application;


namespace WarpDeckForms
{
    public partial class MainForm : Form
    {
        public MainForm(string[] args)
        {
            InitializeComponent();
            //AllocConsole();
            System.Console.SetOut(new TextboxWriter(Console));
            WarpDeckApp app = new(args);
            app.RegisterDependencies();
            app.LoadDevices();
            app.StartPresentation();
            
          
        }

        private void NotifyIcon_OnDoubleClick(object? sender, EventArgs e)
        {
            Show();
            ShowInTaskbar = true;
        }

        private void NotifyIcon_Menu_OpenUI_OnClick(object? sender, EventArgs e)
        {
            
            Process.Start(new ProcessStartInfo("http://localhost:5000") { UseShellExecute = true });
        }

        private void NotifyIcon_Menu_ExitI_OnClick(object? sender, EventArgs e)
        {
            NotifyIcon.Visible = false;
            Application.Exit();
        }

        private void OnFormClosing(object? sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                ShowInTaskbar = false;
                Hide();
                e.Cancel = true;
            }
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();
    }
}