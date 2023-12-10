using System;

namespace COSMIC.Warpdeck.Domain.Device
{
    public class DeviceAlreadyExistsException : Exception
    {
        public DeviceAlreadyExistsException(string deviceId) : base($"Device '{deviceId}' already exists")
        {
        }

    }
}