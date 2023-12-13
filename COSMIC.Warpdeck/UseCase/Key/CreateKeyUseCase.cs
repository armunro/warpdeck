using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.Key;
using COSMIC.Warpdeck.Domain.Property;
using COSMIC.Warpdeck.Managers;

namespace COSMIC.Warpdeck.UseCase.Key
{
    public class CreateDeviceLayerKeyUseCase
    {
        private readonly DeviceManager _deviceManager;

        public CreateDeviceLayerKeyUseCase(DeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        public void Invoke(string deviceId, string layerId, CreateLayerKeyRequestModel model)
        {
            PropertyLookup properties = new PropertyLookup();
            foreach (var modelTag in model.Properties) properties.Add(modelTag.Key, modelTag.Value);

            _deviceManager.GetDevice(deviceId).Layers[layerId].Keys.Add( model.KeyId, new KeyModel
            {
                Behavior = model.Behavior,
                Properties = properties
            });
            
        }
    }
}