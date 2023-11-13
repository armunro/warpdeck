using WarpDeck.Domain.Monitor;
using WarpDeck.Domain.Monitor.Rules;

namespace WarpDeck.Plugins.Monitor.Criteria
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