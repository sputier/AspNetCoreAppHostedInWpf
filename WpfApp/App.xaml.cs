using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WebApp;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;

        public App()
            => _host = Host.CreateDefaultBuilder()
                           .ConfigureServices(services => services.AddHostedService<AspNetAppHostedService>())
                           .Build();

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();
        }

        private async void Application_Exit(object sender, ExitEventArgs e)
        {
            using (_host)
                await _host.StopAsync();
        }
    }

    internal class AspNetAppHostedService : IHostedService
    {
        private IHost _aspNetAppHost;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _aspNetAppHost = Host.CreateDefaultBuilder(null)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                              .UseUrls("https://localhost:12345");
                }).Build();

            await _aspNetAppHost.RunAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            using (_aspNetAppHost)
            {
                // The host application may hang when calling await _aspNetAppHost.StopAsync(); 
                // To prevent it, we need to force the ASP.NET app to die killing its IHostLifetime service :-)
                // See https://github.com/dotnet/extensions/issues/1363 for more informations

                var lifetime = _aspNetAppHost.Services.GetService<IHostLifetime>() as IDisposable;
                lifetime?.Dispose();

                return Task.CompletedTask;
            }
        }
    }
}
