using System;

namespace COSMIC.Warpdeck.Domain.Hardware
{
    public class HardwareNotFoundException : Exception
    {
        public HardwareNotFoundException(string hardwareId) : base($"A device with the hardwareId '{hardwareId}' could not be found.")
        {
            
        }
    }
}