using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Avalonia.ComponentModel;
using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Unity.Microsoft.DependencyInjection;

namespace Radical.Avalonia.Tests.Boot
{
    [TestClass]
    public class RadicalApplicationTestsWithGenericHost
    {
        [AvaloniaTestMethod]
        public async Task Application_can_boot_using_host_builder()
        {
            var bootCompleted = false;

            var host = new HostBuilder()
                .AddRadicalApplication(configuration =>
                {
                    configuration.OnBootCompleted(_ => bootCompleted = true);
                })
                .Build();

            await host.StartAsync();

            var viewResolver = host.Services.GetService<IViewResolver>();

            Assert.IsNotNull(viewResolver);
            Assert.IsTrue(bootCompleted);

            using (host)
            {
                await host?.StopAsync();
            }
        }

        [AvaloniaTestMethod]
        public async Task Application_using_host_builder_can_resolve_configured_services()
        {
            var host = new HostBuilder()
                .ConfigureServices(serviceCollection =>
                {
                    serviceCollection.AddSingleton<SampleDependency>();
                })
                .AddRadicalApplication(_ => { })
                .Build();

            await host.StartAsync();

            var customDependency = host.Services.GetService<SampleDependency>();
            var viewResolver = host.Services.GetService<IViewResolver>();

            Assert.IsNotNull(customDependency);
            Assert.IsNotNull(viewResolver);

            using (host)
            {
                await host?.StopAsync();
            }
        }

        [AvaloniaTestMethod]
        public async Task Application_using_host_builder_can_resolve_application_configuration_configured_services()
        {
            var host = new HostBuilder()
                .AddRadicalApplication(cfg =>
                {
                    cfg.ConfigureServices(services =>
                    {
                        services.AddSingleton<SampleDependency>();
                    });
                })
                .Build();

            await host.StartAsync();

            var customDependency = host.Services.GetService<SampleDependency>();
            var viewResolver = host.Services.GetService<IViewResolver>();

            Assert.IsNotNull(customDependency);
            Assert.IsNotNull(viewResolver);

            using (host)
            {
                await host?.StopAsync();
            }
        }

        [AvaloniaTestMethod]
        public void Application_using_host_builder_can_call_AddRadicalApplication_only_once()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var host = new HostBuilder()
                    .AddRadicalApplication(_ => { })
                    .AddRadicalApplication(_ => { })
                    .Build();
            });
        }

        [AvaloniaTestMethod]
        public async Task Application_using_host_builder_can_call_configure_services_in_any_order()
        {
            var host = new HostBuilder()
                .AddRadicalApplication(_ => { })
                .ConfigureServices(serviceCollection =>
                {
                    serviceCollection.AddSingleton<SampleDependency>();
                })
                .Build();

            await host.StartAsync();

            var customDependency = host.Services.GetService<SampleDependency>();
            var viewResolver = host.Services.GetService<IViewResolver>();

            Assert.IsNotNull(customDependency);
            Assert.IsNotNull(viewResolver);

            using (host)
            {
                await host?.StopAsync();
            }
        }

        [AvaloniaTestMethod]
        public async Task Application_can_boot_using_host_builder_and_Autofac()
        {
            var host = new HostBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .AddRadicalApplication(_ => { })
                .Build();

            await host.StartAsync();

            var resolvedViaHost = host.Services.GetService<IViewResolver>();

            Assert.AreEqual(host.Services.GetType(), typeof(AutofacServiceProvider));
            Assert.IsNotNull(resolvedViaHost);

            using (host)
            {
                await host?.StopAsync();
            }
        }

        [AvaloniaTestMethod]
        public async Task Application_can_boot_using_host_builder_and_Unity()
        {
            var host = new HostBuilder()
                .UseUnityServiceProvider()
                .AddRadicalApplication(_ => { })
                .Build();

            await host.StartAsync();

            var resolvedViaHost = host.Services.GetService<IViewResolver>();

            Assert.AreEqual(host.Services.GetType(), typeof(Unity.Microsoft.DependencyInjection.ServiceProvider));
            Assert.IsNotNull(resolvedViaHost);

            using (host)
            {
                await host?.StopAsync();
            }
        }

        static async Task UsingDesktopLifetime(Func<ClassicDesktopStyleApplicationLifetime, Task> test)
        {
            var lifetime = new ClassicDesktopStyleApplicationLifetime() { ShutdownMode = ShutdownMode.OnExplicitShutdown };
            var previous = RadicalApplication.LifetimeOf;
            RadicalApplication.LifetimeOf = _ => lifetime;
            try
            {
                await test(lifetime);
            }
            finally
            {
                RadicalApplication.LifetimeOf = previous;
            }
        }

        [AvaloniaTestMethod]
        public async Task Stopping_the_host_when_the_lifetime_exits_should_not_shut_down_the_lifetime_again()
        {
            await UsingDesktopLifetime(async lifetime =>
            {
                var host = new HostBuilder()
                    .AddRadicalApplication(_ => { })
                    .Build();

                var exitCount = 0;
                lifetime.Exit += (s, e) =>
                {
                    // the desktop lifetime raises Exit again if shut down while exiting
                    if (++exitCount > 1)
                    {
                        throw new InvalidOperationException("The lifetime has been shut down again while exiting.");
                    }

                    host.StopAsync().GetAwaiter().GetResult();
                };

                await host.StartAsync();
                lifetime.Shutdown();
                host.Dispose();

                Assert.AreEqual(1, exitCount);
            });
        }

        [AvaloniaTestMethod]
        public async Task Stopping_the_host_should_shut_down_the_lifetime()
        {
            await UsingDesktopLifetime(async lifetime =>
            {
                var host = new HostBuilder()
                    .AddRadicalApplication(_ => { })
                    .Build();

                var exitCount = 0;
                lifetime.Exit += (s, e) => exitCount++;

                await host.StartAsync();
                using (host)
                {
                    await host.StopAsync();
                }

                Assert.AreEqual(1, exitCount);
            });
        }
    }
}
