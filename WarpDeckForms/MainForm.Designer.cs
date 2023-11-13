using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;


namespace WarpDeckForms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        protected  TextBox Console = new TextBox();
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
            this.Text = "WarpDeck";
            this.Icon = new Icon("icon.ico");
            this.BackColor = Color.FromArgb(255, 56, 59, 63);
            this.FormClosing += OnFormClosing;
            this.DoubleBuffered = true;
            
            //console
            
            Console.Multiline = true;
            Console.Dock = DockStyle.Fill;
            Console.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Console.Width = this.ClientSize.Width;
            Console.Height = this.ClientSize.Height;
            Console.Padding = new Padding(5);
            Console.BackColor = Color.Black;
            Console.ForeColor = Color.FromArgb(255, 239, 239, 239);
            Console.Font = new Font(new FontFamily("Consolas"), 11f);
            Console.BorderStyle = BorderStyle.None;
            Console.ScrollBars = ScrollBars.Both;
            Controls.Add(Console);

            //NotifyIcon
            NotifyIcon.Icon = this.Icon;
            NotifyIcon.Visible = true;
            NotifyIcon.DoubleClick += NotifyIcon_OnDoubleClick;
            
           
            //NotifyIcon.ContextMenuStrip
            NotifyIcon.ContextMenuStrip = new ContextMenuStrip();
            NotifyIcon.ContextMenuStrip.Items.Add("Open UI", null, NotifyIcon_Menu_OpenUI_OnClick);
            
            NotifyIcon.ContextMenuStrip.Items.Add("Exit", null, NotifyIcon_Menu_ExitI_OnClick);
        }

        #endregion
    }
}