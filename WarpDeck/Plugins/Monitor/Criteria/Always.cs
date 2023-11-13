using WarpDeck.Domain.Monitor;
using WarpDeck.Domain.Monitor.Rules;

namespace WarpDeck.Plugins.Monitor.Criteria
{
    public class Always : MonitorCondition
    {
        public override bool IsMetBy(MonitorChangeEventArgs monitorChange) => true;
    }
}