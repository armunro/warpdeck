using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace COSMIC.Warpdeck.Web
{
    public class WarpDeckFrontend
    {
        public static IContainer Container;


        public static void StartAsync(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<AspNetCoreStartup>())
                .UseServiceProviderFactory(
                    new AutofacChildLifetimeScopeServiceProviderFactory(
                        Container.BeginLifetimeScope("root-one")));
            var app = builder.Build();
            app.Start();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(
                    new AutofacChildLifetimeScopeServiceProviderFactory(
                        Container.BeginLifetimeScope("root-one")))
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<AspNetCoreStartup>());
    }

    public class SwaggerStartup
    {
        public static void ConfigureSwaggerGenOptions(SwaggerGenOptions option)
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "Warpdeck API", Version = "v1" });

            // option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            // {
            //     In = ParameterLocation.Header,
            //     Description = "Please enter a valid token",
            //     Name = "Authorization",
            //     Type = SecuritySchemeType.Http,
            //     BearerFormat = "JWT",
            //     Scheme = "Bearer"
            // });
            //
            // var key = new OpenApiSecurityScheme
            // {
            //     Reference = new OpenApiReference
            //     {
            //         Type = ReferenceType.SecurityScheme,
            //         Id = "Bearer"
            //     }
            // };
            //
            // var requirement = new OpenApiSecurityRequirement { { key, Array.Empty<string>() } };
            //
            // option.AddSecurityRequirement(requirement);
        }
    }
}