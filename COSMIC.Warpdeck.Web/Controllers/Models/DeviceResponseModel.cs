using System.Diagnostics.CodeAnalysis;

namespace COSMIC.Warpdeck.Web.Controllers.Models
{
    [SuppressMessage("ReSharper", "UnusedMember.Global"), SuppressMessage("ReSharper", "UnusedType.Global"),
     SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class DeviceResponseModel
    {
        public string Uri { get; set; }
        public string DeviceId { get; set; }
        public LayerResponseModel[] Layers { get; set; }
       
    }
}