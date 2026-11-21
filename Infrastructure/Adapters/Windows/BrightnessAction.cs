using System;
using Warpdeck.Core.Domain;

namespace Warpdeck.Infrastructure.Adapters.Windows;

public class BrightnessAction : IAction
{
    private readonly IDeviceService _deviceService;
    private readonly string _value;

    public BrightnessAction(IDeviceService deviceService, string value)
    {
        _deviceService = deviceService;
        _value = value?.Trim() ?? string.Empty;
    }

    public void Execute()
    {
        if (string.IsNullOrWhiteSpace(_value)) return;

        if (_value.StartsWith("+") && byte.TryParse(_value.TrimStart('+'), out var addVal))
        {
            int newVal = Math.Min(100, _deviceService.Brightness + addVal);
            _deviceService.SetBrightness((byte)newVal);
        }
        else if (_value.StartsWith("-") && byte.TryParse(_value.TrimStart('-'), out var subVal))
        {
            int newVal = Math.Max(0, _deviceService.Brightness - subVal);
            _deviceService.SetBrightness((byte)newVal);
        }
        else if (byte.TryParse(_value, out var absoluteVal))
        {
            _deviceService.SetBrightness(absoluteVal);
        }
    }
}
