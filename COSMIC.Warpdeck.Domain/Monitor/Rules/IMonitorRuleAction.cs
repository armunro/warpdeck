namespace COSMIC.Warpdeck.Domain.Monitor.Rules
{
    public interface IMonitorRuleAction
    {
        public void ActionWhenTrue();
        public void ActionWhenFalse();
    }
}