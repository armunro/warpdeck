namespace COSMIC.Warpdeck.Domain.Monitor.Rules
{
    public class MonitorRuleCriteriaModel
    {
        public string CriteriaType { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new();
    }
}