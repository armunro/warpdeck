using System.Text.Json.Serialization;

namespace COSMIC.Warpdeck.Domain.Button.Behavior
{
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    // ReSharper disable ClassNeverInstantiated.Global
    public class  BehaviorModel 
    {
        [JsonIgnore]
        public string Type { get; set; } = "PressAndHold";
        public Dictionary<string, Action.ActionModel> Actions { get; set; } = new();
        

    }
}