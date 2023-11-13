using System.Collections.Generic;

namespace WarpDeck.Domain.Monitor.Rules
{
    public class MonitorRuleCriteriaModel
    {
        public string CriteriaType { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new();
    }
}