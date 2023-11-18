using System.Collections.Generic;

namespace COSMIC.Warpdeck.Domain.Monitor
{
    public class MonitorChangeEventArgs
    {
        public Dictionary<string, string> EventData { get; set; } = new();
    }
}