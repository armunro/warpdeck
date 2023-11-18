using System.Collections.Generic;

namespace COSMIC.Warpdeck.Domain.Monitor.Rules
{
    public class MonitorRuleModel
    {
        public string MonitorType { get; set; }
        public MonitorRuleCriteriaModel Criteria { get; set; }
        public List<MonitorRuleActionModel> Actions { get; set; }
    }
}