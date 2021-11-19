using Client.Authentication;
using Client.Contexts;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddSingleton(sp => new AuthenticatingHttpMessageHandler(
                new HttpClientHandler()));

            builder.Services.AddSingleton(sp => new CustomHttpClient(sp.GetRequiredService<AuthenticatingHttpMessageHandler>())
            {
                BaseAddress = new Uri(builder.Configuration["Server"]),
            });

            builder.Services.AddSingleton<SecurityContext>();
            builder.Services.AddSingleton<ComponentContext>();

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            var webHost = builder.Build();
                
            webHost.RunAsync();
        }
    }
}
