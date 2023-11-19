using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace COSMIC.Warpdeck.Web
{
    public class WarpDeckFrontend
    {
        public static IContainer Container;

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(
                    new AutofacChildLifetimeScopeServiceProviderFactory(
                        Container.BeginLifetimeScope("root-one")))
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<AspNetCoreStartup>());

        public static void StartAsync(string[] args) => CreateHostBuilder(args).Build().RunAsync();
    }
}