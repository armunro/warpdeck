using System;

namespace WarpDeck.Domain.Device.Exceptions
{
    public class DeviceNotFoundFoundException : Exception
    {
        public DeviceNotFoundFoundException(string deviceId) : base($"The device '{deviceId}' was not found.")
        {
        }
    }
}