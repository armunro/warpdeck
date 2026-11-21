using Autofac;
using OpenMacroBoard.SDK;
using StreamDeckSharp;
using Warpdeck.Core.Application;
using Warpdeck.Core.Domain;
using Warpdeck.Infrastructure.Adapters.Graphics;
using Warpdeck.Infrastructure.Adapters.Windows;
using Warpdeck.Infrastructure.Adapters.Yaml;

namespace Warpdeck.Infrastructure.Adapters;

public class StreamDeckModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Register Window Service
        builder.RegisterType<WindowsWindowService>()
            .As<IWindowService>()
            .SingleInstance();

        // Register StreamDeck service
        builder.RegisterType<StreamDeckService>()
            .As<IDeviceService>()
            .SingleInstance();

        // Register Graphics Adapter
        builder.RegisterType<SkiaGraphicsAdapter>()
            .As<IGraphicsAdapter>()
            .SingleInstance();

        // Register Layer Manager
        builder.RegisterType<LayerManager>()
            .AsSelf()
            .SingleInstance();

        // Register Layer Provider
        builder.Register(c => new YamlLayerProvider("config.yaml", c.Resolve<IWindowService>(), c.Resolve<IGraphicsAdapter>(), c.Resolve<IDeviceService>()))
            .As<ILayerProvider>()
            .SingleInstance();
    }
}
