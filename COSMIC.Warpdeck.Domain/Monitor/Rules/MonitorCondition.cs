namespace COSMIC.Warpdeck.Domain.Monitor.Rules
{
    public abstract class MonitorCondition
    {
        public abstract bool IsMetBy(MonitorChangeEventArgs monitorChange);
    }
}