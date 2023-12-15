using System.Collections.Generic;
using System.Linq;
using Autofac;
using COSMIC.Warpdeck.Domain.Clipboard;
using COSMIC.Warpdeck.Domain.Configuration;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Managers;
using COSMIC.Warpdeck.Windows.Adapter.Monitor;
using StreamDeckSharp;

namespace COSMIC.Warpdeck.Windows
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WarpdeckWindowsApp
    {
        private readonly string[] _commandLineArgs;
        public static IContainer Container;
        

        public WarpdeckWindowsApp(string[] commandLineArgs)
        {
            _commandLineArgs = commandLineArgs;
        }


        public void RegisterDependencies()
        {
            ContainerBuilder builder = new ContainerBuilder();

            //Register Core dependencies 
            builder.RegisterModule<CoreDependencies.LayersModule>();
            builder.RegisterModule<CoreDependencies.ConfigModule>();
            builder.RegisterModule<CoreDependencies.BehaviorsModule>();
            builder.RegisterModule<CoreDependencies.Property>();
            builder.RegisterModule<CoreDependencies.DevicesModule>();
           
            //Register Windows dependencies
            builder.RegisterModule<WindowsDependencies.BoardModule>();
            builder.RegisterModule<WindowsDependencies.IconsModule>();
            builder.RegisterModule<WindowsDependencies.ActionsModule>();
            builder.RegisterModule<WindowsDependencies.PresentationModule>();
            builder.RegisterModule<WindowsDependencies.MonitorsModule>();
            builder.RegisterModule<WindowsDependencies.ClipboardModule>();

            Container = builder.Build();
            WarpdeckAppContext.Container = Container;
            Web.WarpDeckFrontend.Container = Container;
        }


        public void StartPresentation()
        {
            Web.WarpDeckFrontend.StartAsync(_commandLineArgs);
        }
        
        public void LoadConfig()
        {
            IDeviceReader deviceReader = Container.Resolve<IDeviceReader>();
            var deviceManager = Container.Resolve<DeviceManager>();
            DeviceModelList deviceModels = deviceReader.ReadDevices();
            IEnumerable<IStreamDeckRefHandle> deckRefHandles = StreamDeck.EnumerateDevices();
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
                    deviceManager.BindVirtualDevice(deviceModel);
                }
            }
        }

        public void ReloadConfig()
        {
            Container.Resolve<DeviceManager>().Dispose();
            LoadConfig();
        }

        public void SaveConfig()
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

        public void StartClipboardMonitor()
        {
            Container.Resolve<IClipboardManager>().StartMonitoring();
        }

        public void RegisterWindowsMonitors()
        {
            Container.Resolve<DeviceManager>().AddMonitor(new ActiveWindowMonitor());
        }
    }
}