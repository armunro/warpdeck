using System.Diagnostics.CodeAnalysis;

namespace COSMIC.Warpdeck.Web.Controllers.Models
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class PropertyResponseModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}