namespace Warpdeck.Core.Domain;

public interface IWindowService
{
    WindowInfo? GetForegroundWindowInfo();
}

public record WindowInfo(string ProcessName, string Title);
