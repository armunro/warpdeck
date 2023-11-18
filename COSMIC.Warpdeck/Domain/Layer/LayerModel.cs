using System.Diagnostics.CodeAnalysis;
using COSMIC.Warpdeck.Domain.Key;

namespace COSMIC.Warpdeck.Domain.Layer
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