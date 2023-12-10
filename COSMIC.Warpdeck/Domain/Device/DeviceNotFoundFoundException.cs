using System;

namespace COSMIC.Warpdeck.Domain.Device
{
    public class DeviceNotFoundFoundException : Exception
    {
        public DeviceNotFoundFoundException(string deviceId) : base($"The device '{deviceId}' was not found.")
        {
        }
    }
}