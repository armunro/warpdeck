using System.Collections.Generic;
using System.Linq;
using COSMIC.Warpdeck.Domain.Device.Hardware;

namespace COSMIC.Warpdeck.Adapter.Hardware
{
    public class AttachedHardwareProvider : IHardwareProvider
    {
        public List<HardwareInfo> ProvideAttachedHardwareInfo()
        {
            return StreamDeckSharp.StreamDeck.EnumerateDevices().Select(x => new HardwareInfo()
            {
                HardwareName = x.DeviceName,
                HardwareId = x.DevicePath
            }).ToList();
        }
    }
}