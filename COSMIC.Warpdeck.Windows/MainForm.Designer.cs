using System.Drawing;
using System.Windows.Forms;

namespace COSMIC.Warpdeck.Windows
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        
        protected NotifyIcon NotifyIcon = new NotifyIcon();
        
        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "COSMIC.Warpdeck";
            this.Icon = new System.Drawing.Icon("icon.ico");
            this.BackColor = Color.FromArgb(255, 56, 59, 63);
            this.FormClosing += OnFormClosing;
            this.DoubleBuffered = true;
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;

            //NotifyIcon
            NotifyIcon.Icon = this.Icon;
            NotifyIcon.Visible = true;
            
            //NotifyIcon.ContextMenuStrip
            NotifyIcon.ContextMenuStrip = new ContextMenuStrip();
            NotifyIcon.ContextMenuStrip.Items.Add("Open UI", null, NotifyIcon_Menu_OpenUI_OnClick);
            NotifyIcon.ContextMenuStrip.Items.Add("ReloadConfig", null, NotifyIcon_Menu_Reload_OnClick);
            NotifyIcon.ContextMenuStrip.Items.Add("SaveConfig", null, NotifyIcon_Menu_Save_OnClick);
            NotifyIcon.ContextMenuStrip.Items.Add("Exit", null, NotifyIcon_Menu_ExitI_OnClick);
        }

        #endregion
    }
}