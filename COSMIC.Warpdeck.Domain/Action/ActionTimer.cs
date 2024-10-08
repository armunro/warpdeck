namespace COSMIC.Warpdeck.Domain.Action
{
    public class ActionTimer
    {
        private Dictionary<Tuple<string, int>, Timer> KeyTimers { get; } = new();

        public void RegisterRepeatable(string deviceId, int keyId, int interval, System.Action action)
        {
            var timerKey = new Tuple<string, int>(deviceId, keyId);
            if (KeyTimers.ContainsKey(timerKey))
                UnregisterRepeatable(deviceId, interval);

            KeyTimers[timerKey] = new Timer(_ => action.Invoke(), action,
                TimeSpan.Zero,
                TimeSpan.FromMilliseconds(interval));
        }


        public void UnregisterRepeatable(string deviceId, int keyId)
        {
            var timerKey = new Tuple<string, int>(deviceId, keyId);
            if (KeyTimers.ContainsKey(timerKey))
                KeyTimers[timerKey].Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void UnregisterAllRepeatable()
        {
            foreach (var timersKey in KeyTimers.Keys)
            {
                UnregisterRepeatable(timersKey.Item1, timersKey.Item2);
            }
        }
    }
}