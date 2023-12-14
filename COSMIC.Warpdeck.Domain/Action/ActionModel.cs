

// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace COSMIC.Warpdeck.Domain.Action
{
   
    public class ActionModel
    {
        public string Type { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new();
    }
}