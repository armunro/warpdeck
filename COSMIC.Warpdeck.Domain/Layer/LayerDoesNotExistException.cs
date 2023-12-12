namespace COSMIC.Warpdeck.Domain.Layer
{
    public class LayerDoesNotExistException : Exception
    {
        public LayerDoesNotExistException(string deviceId, string layerId) :
            base($"The layer '{layerId}' doesn't exist on device '{deviceId}'")
        {
        }
    }
}