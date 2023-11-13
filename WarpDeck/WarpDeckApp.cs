using System.Collections.Generic;
using System.Linq;
using Autofac;
using StreamDeckSharp;
using WarpDeck.Domain.Configuration;
using WarpDeck.Domain.Device;
using WarpDeck.UseCase.Device;
using WarpDeck.UseCase.DeviceLayer;

namespace WarpDeck
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