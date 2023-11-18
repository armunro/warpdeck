using System.Collections.Generic;

namespace COSMIC.Warpdeck.Domain.Monitor.Rules
{
    public class MonitorRuleActionModel
    {
        public string ActionType { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new();
    }
}