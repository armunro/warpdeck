namespace WarpDeck.Domain.Monitor
{
    public interface IMonitor
    {
        event MonitorChangeEventDelegate OnMonitorChange;
    }
}