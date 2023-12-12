using COSMIC.Warpdeck.Domain.Monitor;
using COSMIC.Warpdeck.Domain.Monitor.Rules;

namespace COSMIC.Warpdeck.Adapter.Monitor.Criteria
{
    public class AppPathMatches : MonitorCondition
    {
        private readonly string _applicationPath;

        public AppPathMatches(string applicationPath)
        {
            _applicationPath = applicationPath;
        }

        public override bool IsMetBy(MonitorChangeEventArgs monitorChange)
        {
            
            return monitorChange.EventData.ContainsKey("WindowAppPath") &&  monitorChange.EventData["WindowAppPath"].ToLower().Contains(_applicationPath.ToLower());
        }
    }
}