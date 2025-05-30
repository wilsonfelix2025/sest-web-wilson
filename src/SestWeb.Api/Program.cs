using System;
using System.IO;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace SestWeb.Api
{
    public class Program
    {
        /// <summary>
        /// Classe Main
        /// </summary>
        /// <param name="args">Argumentos</param>
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Classe CreateWebHostBuilder
        /// </summary>
        /// <param name="args">Argumentos</param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    IHostingEnvironment env = builderContext.HostingEnvironment;
                    config.AddJsonFile($"autofac.{env.EnvironmentName}.json");
                    config.AddJsonFile("appsettings.json");
                    config.AddEnvironmentVariables();
                })
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                    loggerConfiguration.MinimumLevel.Debug()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                        .Enrich.FromLogContext()
                        .WriteTo.File(Path.Combine(hostingContext.HostingEnvironment.ContentRootPath, "logs/log-.log"), rollingInterval: RollingInterval.Day);
                })
                .UseKestrel(option =>
                {
                    option.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
                    option.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(10);
                })
                .UseIISIntegration()
                .ConfigureServices(services => services.AddAutofac())
                .UseUrls("http://0.0.0.0:5000");
        }
    }
}
