using System.Collections.Generic;

namespace COSMIC.Warpdeck.Domain.Device.Hardware
{
    public interface IHardwareProvider
    {
        List<HardwareInfo> ProvideAttachedHardwareInfo();
    }
}