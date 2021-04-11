using DotNetForHtml5;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace TestApplication.OpenSilver.Browser
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            Cshtml5Initializer.Initialize();
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            var host = builder.Build();
            await host.RunAsync();
        }

        public static void RunApplication()
        {
            Application.RunApplication(() =>
                {
                    var app = new TestApplication.App();
                });
        }
    }
}
