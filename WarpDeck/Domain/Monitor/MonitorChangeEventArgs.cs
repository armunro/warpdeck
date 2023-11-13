using System.Collections.Generic;

namespace WarpDeck.Domain.Monitor
{
    public class MonitorChangeEventArgs
    {
        public Dictionary<string, string> EventData { get; set; } = new();
    }
}