using COSMIC.Warpdeck.Domain.Key.Action;

namespace COSMIC.Warpdeck.Domain.Key.Behavior
{
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    // ReSharper disable ClassNeverInstantiated.Global
    public class  BehaviorModel 
    {
        public string Type { get; set; }
        public Dictionary<string, ActionModel> Actions { get; set; } = new();
        

    }
}