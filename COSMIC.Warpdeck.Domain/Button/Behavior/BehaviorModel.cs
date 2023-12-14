namespace COSMIC.Warpdeck.Domain.Button.Behavior
{
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    // ReSharper disable ClassNeverInstantiated.Global
    public class  BehaviorModel 
    {
        public string Type { get; set; }
        public Dictionary<string, Action.ActionModel> Actions { get; set; } = new();
        

    }
}