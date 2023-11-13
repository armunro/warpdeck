using System.Collections.Generic;

namespace WarpDeck.Domain.Monitor.Rules
{
    public class MonitorRule
    {
        public MonitorCondition Criteria { get; set; }
        public List<IMonitorRuleAction> Actions { get; set; } = new();
    }
}