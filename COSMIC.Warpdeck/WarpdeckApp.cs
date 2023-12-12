using System.Collections.Generic;
using System.Linq;
using Autofac;
using COSMIC.Warpdeck.Domain.Clipboard;
using COSMIC.Warpdeck.Domain.Configuration;
using COSMIC.Warpdeck.Domain.Device;
using StreamDeckSharp;

namespace COSMIC.Warpdeck
{
    public class WarpdeckApp
    {
        public static IContainer Container;

 
        public void LoadClipboardPatterns()
        {
            
        }
        
        public void LoadDevices()
        {
            IDeviceReader deviceReader = Container.Resolve<IDeviceReader>();
            var deviceManager = Container.Resolve<DeviceManager>();
            DeviceModelList deviceModels = deviceReader.ReadDevices();
            List<IStreamDeckRefHandle> deckRefHandles = StreamDeck.EnumerateDevices().ToList();
            foreach (DeviceModel deviceModel in deviceModels)
            {
                if (deviceModel.Info.HardwareId != "virtual")
                {
                    var hardwareMatch = deckRefHandles.FirstOrDefault(x => x.DevicePath == deviceModel.Info.HardwareId);
                    if (hardwareMatch != null)
                    {
                        deviceManager.BindDevice(hardwareMatch.Open(), deviceModel);
                    }
                }
                else
                {
                    deviceManager.BindDevice(deviceModel);
                }

                //Container.Resolve<RedrawDeviceLayersUseCase>().Invoke(deviceModel.DeviceId);
            }
        }

        public void Reload()
        {
            Container.Resolve<DeviceManager>().Dispose();
            LoadDevices();
        }

        public void Save()
        {
            var clipPatternWriter = Container.Resolve<IClipPatternWriter>();
            clipPatternWriter.WritePatterns(Container.Resolve<IClipboardManager>().Patterns);
            
            var deviceManager = Container.Resolve<DeviceManager>();
            var deviceWriter = Container.Resolve<IDeviceWriter>();
            foreach (var deviceModel in deviceManager.GetAllDevices())
            {
                deviceWriter.WriteDeviceModel(deviceModel);
            }
        }
    }
}