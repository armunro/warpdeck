using System.ComponentModel;
using Microsoft.Web.WebView2.WinForms;

namespace COSMIC.Warpdeck.Windows;

partial class DeviceHost
{

    private IContainer components = null;
    WebView2 _deviceHostWebView = null;

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
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.Icon = new System.Drawing.Icon("icon.ico");
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Text = "DeviceHost";
        
        this._deviceHostWebView = new WebView2();
        this._deviceHostWebView.Dock = System.Windows.Forms.DockStyle.Fill;
        
        Controls.Add(_deviceHostWebView);
    }

    #endregion
}