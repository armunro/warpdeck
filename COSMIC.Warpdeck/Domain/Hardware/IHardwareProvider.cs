using System.Collections.Generic;

namespace COSMIC.Warpdeck.Domain.Hardware
{
    public interface IHardwareProvider
    {
        List<HardwareInfo> ProvideAttachedHardwareInfo();
    }
}