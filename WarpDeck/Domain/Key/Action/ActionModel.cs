using System.Collections.Generic;

// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace WarpDeck.Domain.Key.Action
{
   
    public class ActionModel
    {
        public string Type { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new();
    }
}