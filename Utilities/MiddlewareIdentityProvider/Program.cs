using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiddlewareIdentityProvider.Data;
using Serilog;

namespace MiddlewareIdentityProvider
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope()) {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureLogging(configureLogging => configureLogging.ClearProviders());
                    webBuilder.UseSerilog((webHostBuilderContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(webHostBuilderContext.Configuration));
                    webBuilder.UseStartup<Startup>();
                });
    }
}
