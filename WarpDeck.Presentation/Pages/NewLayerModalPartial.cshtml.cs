using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WarpDeck.Presentation.Pages
{
    public class NewLayerModalPartial : PageModel
    {
        public void OnGet()
        {
            DeviceId = RouteData.Values["deviceId"]?.ToString();
        }

        public string DeviceId { get; set; }
    }
}