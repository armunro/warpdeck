using System.Collections.Generic;
using Autofac;
using COSMIC.Warpdeck.Domain.Clipboard;
using Microsoft.AspNetCore.Mvc;

namespace COSMIC.Warpdeck.Web.Controllers
{
    [ApiController]
    public class ClipboardController : Controller
    {
        [HttpGet, Route("/api/clipboard")]
        public List<Clip>  GetClips()
        {
            var clipboardManager = WarpDeckFrontend.Container.Resolve<IClipboardManager>();
            return clipboardManager.GetClips();
        }
    }
}