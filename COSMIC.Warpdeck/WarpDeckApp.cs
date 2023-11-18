using System.Collections.Generic;
using System.Linq;
using Autofac;
using COSMIC.Warpdeck.Domain.Configuration;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.UseCase.DeviceLayer;
using StreamDeckSharp;
using COSMIC.Warpdeck.UseCase.Device;

namespace COSMIC.Warpdeck
{
    public class WarpDeckApp
    {
        public static IContainer Container;


        public void LoadDevices()
        {
            IDeviceReader deviceReader = Container.Resolve<IDeviceReader>();

            var deviceManager = Container.Resolve<DeviceManager>();
            DeviceModelList deviceModels = deviceReader.ReadDevices();

            List<IStreamDeckRefHandle> deckRefHandles = StreamDeck.EnumerateDevices().ToList();

            var deviceModel = deviceModels.First();
            foreach (IStreamDeckRefHandle streamDeckRefHandle in deckRefHandles)
            {
                deviceModel.Info.HardwareId = streamDeckRefHandle.DevicePath;
                deviceManager.BindDevice(streamDeckRefHandle.Open(), deviceModel);
            }

            Container.Resolve<RedrawDeviceLayersUseCase>().Invoke(deviceModel.DeviceId);
            
        }

        public void Reload()
        {
            Container.Resolve<DeviceManager>().Dispose();
            LoadDevices();
        }

        public void Save()
        {
            var deviceManager = Container.Resolve<DeviceManager>();
            var deviceWriter = Container.Resolve<IDeviceWriter>();
            foreach (var deviceModel in deviceManager.GetAllDevices())
            {
                deviceWriter.WriteDeviceModel(deviceModel);
            }
        }
    }
}