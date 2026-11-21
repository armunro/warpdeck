using System;
using System.Windows;
using Autofac;
using Warpdeck.Core.Application;
using Warpdeck.Core.Domain;
using Warpdeck.Infrastructure.Adapters;
using Warpdeck.UI.ViewModels;
using Warpdeck.UI.Views;

namespace Warpdeck;

public partial class App : Application
{
    private IContainer? _container;
    private ILifetimeScope? _scope;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        if (Environment.GetCommandLineArgs().Contains("--test") || (e.Args != null && e.Args.Contains("--test")))
        {
            TestNamespace.TestRecording.RunTests();
            Shutdown(0);
            return;
        }

        var builder = new ContainerBuilder();
        builder.RegisterModule<StreamDeckModule>();

        // Register ViewModels & Views
        builder.RegisterType<MainViewModel>().AsSelf().SingleInstance();
        builder.RegisterType<MainWindow>().AsSelf().SingleInstance();

        _container = builder.Build();
        _scope = _container.BeginLifetimeScope();

        var deviceService = _scope.Resolve<IDeviceService>();
        var layerManager = _scope.Resolve<LayerManager>();
        var layerProvider = _scope.Resolve<ILayerProvider>();

        // Start scanning for Stream Deck hardware
        deviceService.Start();

        // Initial layer loading for hardware
        layerManager.SetLayers(layerProvider.GetLayers());

        // Dynamic layer updates
        layerProvider.LayersChanged += () =>
        {
            layerManager.SetLayers(layerProvider.GetLayers());
        };

        var mainWindow = _scope.Resolve<MainWindow>();
        var mainVm = _scope.Resolve<MainViewModel>();
        mainWindow.DataContext = mainVm;
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _scope?.Dispose();
        _container?.Dispose();
        base.OnExit(e);
    }
}
