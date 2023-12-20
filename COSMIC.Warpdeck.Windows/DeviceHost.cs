using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace COSMIC.Warpdeck.Windows;

public partial class DeviceHost : Form
{
    public DeviceHost()
    {
        InitializeComponent();
    }

    public async void LoadDeviceHost(string deviceId)
    {
        await _deviceHostWebView.EnsureCoreWebView2Async();
        _deviceHostWebView.CoreWebView2.Navigate($"http://localhost:4300/deviceBare/{deviceId}/");
        
        await SetFormSizeAfter3SecondDelay();
    }

    private async void SetFormSizeToMatchKeyGrid()
    {
        string script = @"
    (function() {
        var element = document.getElementById('keymapGrid');
        return {
            width: element.offsetWidth,
            height: element.offsetHeight
        };
    })();";

        string result = await _deviceHostWebView.ExecuteScriptAsync(script);
        // The result is a JSON string like {"width":500,"height":500}
        var size = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(result);
        int width = size["width"];
        int height = size["height"];
        Width = width + 40;
        Height = height + 40;
    }
    private async void RemoveScrollbars()
    {
        string script = @"
    (function() {
        var style = document.createElement('style');
        style.innerHTML = `
        ::-webkit-scrollbar { 
            display: none; 
        }`;
        document.head.appendChild(style);
    })();";

        await _deviceHostWebView.ExecuteScriptAsync(script);
    }
    private async Task SetFormSizeAfter3SecondDelay()
    {
        await Task.Delay(3000);
        SetFormSizeToMatchKeyGrid();
        RemoveScrollbars();
    }
}