using System.Collections.Generic;
using COSMIC.Warpdeck.Domain.Monitor;

namespace COSMIC.Warpdeck.Adapter.Monitor
{
    public class AdHocMonitor : IMonitor
    {
        public event MonitorChangeEventDelegate OnMonitorChange;

        public void Fire(Dictionary<string,string> data)
        {
            OnMonitorChange?.Invoke(this, new MonitorChangeEventArgs(){EventData = data});
        }
    }
}