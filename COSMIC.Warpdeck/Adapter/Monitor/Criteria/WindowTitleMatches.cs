using COSMIC.Warpdeck.Domain.Monitor;
using COSMIC.Warpdeck.Domain.Monitor.Rules;

namespace COSMIC.Warpdeck.Adapter.Monitor.Criteria
{
    public class WindowTitleMatches : MonitorCondition
    {
        private readonly string _windowTitle;

        public WindowTitleMatches(string windowTitle)
        {
            _windowTitle = windowTitle;
        }

        public override bool IsMetBy(MonitorChangeEventArgs monitorChange)
        {
            return monitorChange.EventData["WindowTitle"].Contains(_windowTitle);
        }
    }
}