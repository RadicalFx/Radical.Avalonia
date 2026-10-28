using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Avalonia.ComponentModel;
using System;
using Avalonia;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Radical.Avalonia.Tests.Boot
{
    [TestClass]
    public class RadicalApplicationTests
    {
        [AvaloniaTestMethod]
        public void ApplicationBoot_process_should_create_valid_container()
        {
            var bootCompleted = false;
            IServiceProvider container = null;

            var radicalApplication = Application.Current.AddRadicalApplication(configuration =>
            {
                configuration.DisableAutoBoot();
                configuration.OnServiceProviderCreated(serviceProvider => container = serviceProvider);
                configuration.OnBootCompleted(_ => bootCompleted = true);
            });

            radicalApplication.Boot();

            var viewResolver = container.GetService<IViewResolver>();

            Assert.IsNotNull(viewResolver);
            Assert.IsTrue(bootCompleted);
        }

        [AvaloniaTestMethod]
        public void ApplicationBoot_process_should_allow_services_customization()
        {
            var bootCompleted = false;
            IServiceProvider container = null;

            var radicalApplication = Application.Current.AddRadicalApplication(configuration =>
            {
                configuration.ConfigureServices(services =>
                {
                    services.AddTransient<SampleDependency>();
                });
                configuration.DisableAutoBoot();
                configuration.OnServiceProviderCreated(serviceProvider => container = serviceProvider);
                configuration.OnBootCompleted(_ => bootCompleted = true);
            });

            radicalApplication.Boot();

            var service = container.GetService<SampleDependency>();

            Assert.IsNotNull(service);
            Assert.IsTrue(bootCompleted);
        }
    }
}
