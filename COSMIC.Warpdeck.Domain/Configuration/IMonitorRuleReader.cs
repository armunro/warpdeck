using COSMIC.Warpdeck.Domain.Monitor.Rules;

namespace COSMIC.Warpdeck.Domain.Configuration;

public interface IMonitorRuleReader
{
    MonitorRuleList ReadMonitorRules();
}