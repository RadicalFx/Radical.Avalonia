using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Radical.Avalonia.Bootstrap;
using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;

namespace Radical.Avalonia.Hosting
{
    class RadicalApplicationService : IHostedService
    {
        IServiceProvider serviceProvider;
        BootstrapConfiguration bootstrapConfiguration;
        RadicalApplication radicalApplication;

        public RadicalApplicationService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            bootstrapConfiguration = serviceProvider.GetRequiredService<BootstrapConfiguration>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var application = serviceProvider.GetRequiredService<Application>();
            radicalApplication = RadicalApplication.ExternallyManagedBy(application, serviceProvider, bootstrapConfiguration);
            radicalApplication.BootApplication();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            radicalApplication.Shutdown();
            return Task.CompletedTask;
        }
    }
}