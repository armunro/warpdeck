using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading.Tasks;
using COSMIC.Warpdeck.Domain.Device;

namespace COSMIC.Warpdeck.Windows;

public partial class ClipboardHostForm : Form
{
    public ClipboardHostForm()
    {
        InitializeComponent();
        LoadClipboardHost();
        this.Shown += (sender, args) => SetFormStartLocation();
    }

    private void SetFormStartLocation()
    {
        var screen = Screen.PrimaryScreen;
        Left = screen.WorkingArea.Width - Width;
        Top = screen.WorkingArea.Height - Height;
    }

    public async void LoadClipboardHost()
    {
        Activate();
        BringToFront();
        TopLevel = true;
        TopMost = true;
        await _clipboardHostWebView.EnsureCoreWebView2Async();
        _clipboardHostWebView.CoreWebView2.Navigate($"http://localhost:4300/clipboard");

        // Add this line to remove horizontal scroll bar
        await _clipboardHostWebView.CoreWebView2.ExecuteScriptAsync("document.documentElement.style.overflowX = 'hidden';");
    }
}