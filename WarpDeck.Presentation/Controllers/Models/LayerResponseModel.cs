using System.Diagnostics.CodeAnalysis;

namespace WarpDeck.Presentation.Controllers.Models
{
    [SuppressMessage("ReSharper", "UnusedMember.Global"), SuppressMessage("ReSharper", "UnusedType.Global"),
     SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class LayerResponseModel
    {
        public string Uri { get; set; }
        public KeyResponseModel[] Keys { get; set; }
    }
}