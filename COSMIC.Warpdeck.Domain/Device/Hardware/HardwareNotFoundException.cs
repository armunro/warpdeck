namespace COSMIC.Warpdeck.Domain.Device.Hardware
{
    public class HardwareNotFoundException : Exception
    {
        public HardwareNotFoundException(string hardwareId) : base($"A device with the hardwareId '{hardwareId}' could not be found.")
        {
            
        }
    }
}