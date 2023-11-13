using System.Collections.Generic;

namespace WarpDeck.Domain.Hardware
{
    public interface IHardwareProvider
    {
        List<HardwareInfo> ProvideAttachedHardwareInfo();
    }
}