namespace WarpDeck.Domain.Monitor.Rules
{
    public interface IMonitorRuleAction
    {
        public void ActionWhenTrue();
        public void ActionWhenFalse();
    }
}