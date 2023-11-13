using System.Diagnostics.CodeAnalysis;
using WarpDeck.Domain.Key;

namespace WarpDeck.Domain.Layer
{
    [SuppressMessage("ReSharper", "UnusedMember.Global"), SuppressMessage("ReSharper", "UnusedType.Global"),
     SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class LayerModel
    {
        public string LayerId { get; set; }
        public KeyMap Keys { get; set; } = new();
        public int Level { get; set; } = 0;
    }
}