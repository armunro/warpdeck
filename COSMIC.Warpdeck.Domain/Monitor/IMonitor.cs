namespace COSMIC.Warpdeck.Domain.Monitor
{
    public interface IMonitor
    {
        event MonitorChangeEventDelegate OnMonitorChange;
    }
}