using Avalonia;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Avalonia.Bootstrap;
using Radical.Avalonia.ComponentModel;
using Radical.Avalonia.Presentation;
using Radical.Avalonia.Tests.Boot.Presentation;
using System;
using System.Threading.Tasks;

namespace Radical.Avalonia.Tests.Boot
{
    [TestClass]
    public class RadicalApplicationSplashScreenTests
    {
        [AvaloniaTestMethod]
        public void ApplicationBoot_process_should_show_the_splash_screen_and_then_the_shell()
        {
            IServiceProvider container = null;
            var startupWorkCompleted = false;

            var radicalApplication = Application.Current.AddRadicalApplication<MainView>(configuration =>
            {
                configuration.DisableAutoBoot();
                configuration.OnServiceProviderCreated(serviceProvider => container = serviceProvider);
                configuration.EnableSplashScreen(new SplashScreenConfiguration()
                {
                    SplashScreenViewType = typeof(TestSplashScreenView),
                    MinimumDelay = 0,
                    StartupAsyncWork = _ =>
                    {
                        startupWorkCompleted = true;
                        return Task.CompletedTask;
                    }
                });
            });

            radicalApplication.Boot();

            var splashScreen = TestSplashScreenView.LastCreated;
            var shell = container.GetRequiredService<MainView>();
            try
            {
                Assert.AreEqual(WindowDecorations.None, splashScreen.WindowDecorations);

                var shellShown = UIThread.RunJobsUntil(() => shell.IsVisible && !splashScreen.IsVisible);

                Assert.IsTrue(startupWorkCompleted);
                Assert.IsTrue(shellShown, "The shell was not shown or the splash screen was not closed.");
            }
            finally
            {
                shell.Close();
                splashScreen.Close();
            }
        }

        [AvaloniaTestMethod]
        public void Default_splash_screen_view_should_host_the_splash_screen_content_region()
        {
            IServiceProvider container = null;
            var radicalApplication = Application.Current.AddRadicalApplication(configuration =>
            {
                configuration.DisableAutoBoot();
                configuration.OnServiceProviderCreated(serviceProvider => container = serviceProvider);
            });
            radicalApplication.Boot();

            var view = container.GetRequiredService<IViewResolver>().GetView<SplashScreenView>();
            var regionService = container.GetRequiredService<IRegionService>();

            Assert.IsTrue(regionService.HoldsRegionManager(view));
            Assert.IsTrue(regionService.GetRegionManager(view).TryGetRegion<IContentRegion>("SplashScreenContent", out _));
        }
    }
}
