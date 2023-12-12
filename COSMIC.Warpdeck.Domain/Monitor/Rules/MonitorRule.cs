namespace COSMIC.Warpdeck.Domain.Monitor.Rules
{
    public class MonitorRule
    {
        public MonitorCondition Criteria { get; set; }
        public List<IMonitorRuleAction> Actions { get; set; } = new();
    }
}