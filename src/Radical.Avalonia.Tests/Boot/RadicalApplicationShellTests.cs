using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Avalonia.Tests.Boot.Presentation;
using System;

namespace Radical.Avalonia.Tests.Boot
{
    [TestClass]
    public class RadicalApplicationShellTests
    {
        [AvaloniaTestMethod]
        public void ApplicationBoot_process_should_show_the_shell()
        {
            IServiceProvider container = null;

            var radicalApplication = Application.Current.AddRadicalApplication<MainView>(configuration =>
            {
                configuration.DisableAutoBoot();
                configuration.OnServiceProviderCreated(serviceProvider => container = serviceProvider);
            });

            radicalApplication.Boot();

            var shell = container.GetRequiredService<MainView>();
            try
            {
                Assert.IsTrue(shell.IsVisible);
            }
            finally
            {
                shell.Close();
            }
        }
    }
}
