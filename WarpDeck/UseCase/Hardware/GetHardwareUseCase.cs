using System.Collections.Generic;
using System.Linq;
using WarpDeck.Domain.Device;
using WarpDeck.Domain.Hardware;
using WarpDeck.UseCase.Device;

namespace WarpDeck.UseCase.Hardware
{
    public class GetHardwareUseCase
    {
        private readonly IHardwareProvider _boardProvider;
        private readonly DeviceManager _deviceManager;

        public GetHardwareUseCase(IHardwareProvider boardProvider, DeviceManager deviceManager)
        {
            _boardProvider = boardProvider;
            _deviceManager = deviceManager;
        }

        public List<HardwareInfo> Invoke(bool onlyAvailable)
        {
            var availableBoards = _boardProvider.ProvideAttachedHardwareInfo();
            if (!onlyAvailable)
                return availableBoards;
            
            IEnumerable<string> registeredIdentifiers = _deviceManager.GetAllDevices().Select(x => x.Info.HardwareId);
            return availableBoards.Where(x => !registeredIdentifiers.Contains(x.HardwareId)).ToList();
        } 
    }
}