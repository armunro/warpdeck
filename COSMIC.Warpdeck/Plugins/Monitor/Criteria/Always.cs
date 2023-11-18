using COSMIC.Warpdeck.Domain.Monitor;
using COSMIC.Warpdeck.Domain.Monitor.Rules;

namespace COSMIC.Warpdeck.Plugins.Monitor.Criteria
{
    public class Always : MonitorCondition
    {
        public override bool IsMetBy(MonitorChangeEventArgs monitorChange) => true;
    }
}