using COSMIC.Warpdeck.Domain.Button;
using COSMIC.Warpdeck.Domain.Device;
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

        public void Invoke(string deviceId, string layerId, CreateLayerButtonRequestModel model)
        {
            PropertyLookup properties = new PropertyLookup();
            foreach (var modelTag in model.Properties) properties.Add(modelTag.Key, modelTag.Value);

            _deviceManager.GetDevice(deviceId).Layers[layerId].Buttons.Add(model.KeyId, new ButtonModel
            {
                Properties = properties
            });
            
        }
    }
}