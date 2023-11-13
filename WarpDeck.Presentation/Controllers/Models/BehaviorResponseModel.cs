using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace WarpDeck.Presentation.Controllers.Models
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class BehaviorResponseModel
    {
        public string Uri { get; set; }
        public string BehaviorId { get; set; }
        public List<string> Actions { get; set; }
    }
}