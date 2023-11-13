using System;

namespace WarpDeck.Domain.Device.Exceptions
{
    public class DeviceAlreadyExistsException : Exception
    {
        public DeviceAlreadyExistsException(string deviceId) : base($"Device '{deviceId}' already exists")
        {
        }

    }
}