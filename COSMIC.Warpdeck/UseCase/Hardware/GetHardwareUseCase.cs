using System.Collections.Generic;
using System.Linq;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.Device.Hardware;
using COSMIC.Warpdeck.UseCase.Device;

namespace COSMIC.Warpdeck.UseCase.Hardware
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