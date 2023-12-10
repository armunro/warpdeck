using System.Collections.Generic;
using COSMIC.Warpdeck.Domain.Device.Hardware;
using Microsoft.AspNetCore.Mvc.RazorPages;
using COSMIC.Warpdeck.UseCase.Hardware;

namespace COSMIC.Warpdeck.Web.Pages
{
    public class NewDevice : PageModel
    {
        private readonly GetHardwareUseCase _getHardware;
        
        public List<HardwareInfo> AvailableBoards;

        public NewDevice(GetHardwareUseCase getHardware)
        {
            _getHardware = getHardware;
        }

        public void OnGet()
        {
            AvailableBoards = _getHardware.Invoke(true);
        }
    }
}