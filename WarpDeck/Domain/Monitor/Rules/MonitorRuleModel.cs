using System.Collections.Generic;

namespace WarpDeck.Domain.Monitor.Rules
{
    public class MonitorRuleModel
    {
        public string MonitorType { get; set; }
        public MonitorRuleCriteriaModel Criteria { get; set; }
        public List<MonitorRuleActionModel> Actions { get; set; }
    }
}