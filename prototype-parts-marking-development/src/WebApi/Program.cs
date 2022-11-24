namespace WebApi
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using Autofac.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using Serilog.Exceptions;
    using Serilog.Formatting.Compact;
    using WebApi.Common.Logging;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(o =>
                    {
                        var config = o.ApplicationServices.GetRequiredService<IConfiguration>();

                        var endpoints = config.GetSection("HttpServer:Endpoints")
                            .GetChildren()
                            .ToDictionary(s => s.Key, s =>
                            {
                                var endpoint = new EndpointConfiguration();
                                s.Bind(endpoint);
                                return endpoint;
                            });

                        foreach (var (_, endpoint) in endpoints)
                        {
                            var ipAddresses = new List<IPAddress>();
                            if (endpoint.Host == "localhost")
                            {
                                ipAddresses.Add(IPAddress.IPv6Loopback);
                                ipAddresses.Add(IPAddress.Loopback);
                            }
                            else if (IPAddress.TryParse(endpoint.Host, out var address))
                            {
                                ipAddresses.Add(address);
                            }
                            else
                            {
                                ipAddresses.Add(IPAddress.IPv6Any);
                            }

                            foreach (var ipAddress in ipAddresses)
                            {
                                o.Listen(ipAddress, endpoint.Port, listenOptions =>
                                {
                                    if (endpoint.Scheme == "https")
                                    {
                                        listenOptions.UseHttps(new X509Certificate2(
                                            endpoint.CertPath,
                                            endpoint.CertPassword));
                                    }
                                });
                            }
                        }
                    });

                    webBuilder.UseWebRoot(Path.Combine(AppContext.BaseDirectory, "wwwroot"));
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog((context, logger) =>
                {
                    logger
                        .Destructure.With<DestructurablePolicy>()
                        .ReadFrom.Configuration(context.Configuration)
                        .Enrich.WithExceptionDetails()
                        .Enrich.FromLogContext()
                        .WriteTo.Async(a
                            => a.Console(
                                outputTemplate:
                                "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{Properties:j}{NewLine}"))
                        .WriteTo.Async(a => a.File(
                            new CompactJsonFormatter(),
                            Path.Combine(AppContext.BaseDirectory, "logs", "log.json"),
                            rollingInterval: RollingInterval.Hour,
                            retainedFileCountLimit: 24));
                })
                .UseWindowsService();
        }
    }
}
