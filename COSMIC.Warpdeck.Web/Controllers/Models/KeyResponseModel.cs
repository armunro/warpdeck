using System.Diagnostics.CodeAnalysis;

namespace COSMIC.Warpdeck.Web.Controllers.Models
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class KeyResponseModel
    {
        public string Uri { get; set; }
        public int KeyId { get; set; }
        public BehaviorResponseModel Behavior { get; set; }
        public string IconUri { get; set; }
        public PropertyResponseModel[] Properties { get; set; }
    }
}