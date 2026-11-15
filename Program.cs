using Autofac;
using OpenMacroBoard.SDK;
using Warpdeck.Core.Application;
using Warpdeck.Core.Domain;
using Warpdeck.Infrastructure.Adapters;

var builder = new ContainerBuilder();
builder.RegisterModule<StreamDeckModule>();
using var container = builder.Build();

using var scope = container.BeginLifetimeScope();
var deviceService = scope.Resolve<IDeviceService>();
var layerManager = scope.Resolve<LayerManager>();
var layerProvider = scope.Resolve<ILayerProvider>();

// Start scanning for device
deviceService.Start();

// Set initial layers
layerManager.SetLayers(layerProvider.GetLayers());

// Subscribe to reloads
layerProvider.LayersChanged += () =>
{
    layerManager.SetLayers(layerProvider.GetLayers());
};

Console.WriteLine("StreamDeck Layer System active (Autofac DI).");
Console.WriteLine("Running in background loop. Close window or stop via Catalyst to exit.");

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

while (!cts.Token.IsCancellationRequested)
{
    Thread.Sleep(1000);
}
