using System.Collections.Generic;

namespace WarpDeck.Domain.Monitor.Rules
{
    public class MonitorRuleList
    {
        public List<MonitorRuleModel> Rules { get; set; } = new();
    }
}