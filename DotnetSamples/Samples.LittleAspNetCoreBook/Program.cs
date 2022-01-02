using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Samples.LittleAspNetCoreBook
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            InitializeDatabase(host)    ;
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });


        private static void InitializeDatabase(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    SeedData.InitializeAsync(services).Wait();
                }
                catch (Exception ex)
                {
                    var logger = services
                        .GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Error occurred seeding the DB.");
                }
            }
        }
    }
}
