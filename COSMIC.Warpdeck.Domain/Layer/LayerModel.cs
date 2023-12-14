using System.Diagnostics.CodeAnalysis;
using COSMIC.Warpdeck.Domain.Button;

namespace COSMIC.Warpdeck.Domain.Layer
{
    [SuppressMessage("ReSharper", "UnusedMember.Global"), SuppressMessage("ReSharper", "UnusedType.Global"),
     SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class LayerModel
    {
        public string LayerId { get; set; }
        public  ButtonMap Buttons { get; set; } = new();
        public int Level { get; set; } = 0;
    }
}