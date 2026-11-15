using Warpdeck.Core.Domain;

namespace Warpdeck.Infrastructure.Adapters.Windows;

public class WindowCondition : ICondition
{
    private readonly IWindowService _windowService;
    private readonly string? _processName;
    private readonly string? _windowTitle;

    public WindowCondition(IWindowService windowService, string? processName = null, string? windowTitle = null)
    {
        _windowService = windowService;
        _processName = processName;
        _windowTitle = windowTitle;
    }

    public bool IsMet()
    {
        var info = _windowService.GetForegroundWindowInfo();
        if (info == null) return false;

        if (_processName != null)
        {
            if (!string.Equals(info.ProcessName, _processName, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        if (_windowTitle != null)
        {
            if (!info.Title.Contains(_windowTitle, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }
}
