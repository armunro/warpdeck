using System;
using System.IO;
using System.Text.Json.Serialization;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace COSMIC.Warpdeck.Web
{
    public class AspNetCoreStartup
    {
        private IConfiguration Configuration { get; }


        public AspNetCoreStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages()
                .WithRazorPagesRoot(Defaults.RazorPagesRoot)
                .AddJsonOptions(
                    options => options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                );
            services.AddEndpointsApiExplorer();
            services.AddControllers();
            services.AddSwaggerGen();
            
        }

        public void ConfigureContainer(AutofacChildLifetimeScopeConfigurationAdapter config)
        {
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = "",
                FileProvider =
                    new PhysicalFileProvider(Path.Combine(Environment.CurrentDirectory, Defaults.PresentationDirectory))
            });
            app.UseRouting();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUI(options => options.EnableTryItOutByDefault());
        }
    }
}