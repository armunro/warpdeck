using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WarpDeck.Domain.Hardware;
using WarpDeck.UseCase.Hardware;

namespace WarpDeck.Presentation.Pages
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