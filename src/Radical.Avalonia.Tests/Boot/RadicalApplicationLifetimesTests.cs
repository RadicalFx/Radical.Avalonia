using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Avalonia.Bootstrap;
using Radical.Avalonia.Tests.Boot.Presentation;
using System;
using System.Threading.Tasks;

namespace Radical.Avalonia.Tests.Boot
{
    [TestClass]
    public class RadicalApplicationLifetimesTests
    {
        /*
         * Single-view lifetimes cannot be implemented outside Avalonia:
         * the shell host is created on top of the recorded main view instead.
         */
        Control mainView;
        Func<Control> mainViewFactory;
        Func<IApplicationLifetime, ShellHost> previousFactory;

        [TestInitialize]
        public void Setup()
        {
            previousFactory = ShellHost.Factory;
        }

        [TestCleanup]
        public void Cleanup()
        {
            ShellHost.Factory = previousFactory;
        }

        void UseSingleViewLifetime()
        {
            ShellHost.Factory = _ => new SingleViewShellHost(view => mainView = view);
        }

        void UseActivityLifetime()
        {
            ShellHost.Factory = _ => new ActivityShellHost(factory => mainViewFactory = factory);
        }

        static (RadicalApplication, Func<IServiceProvider>) CreateApplication<TShellView>(bool enableSplashScreen = false) where TShellView : Control
        {
            IServiceProvider container = null;
            var radicalApplication = Application.Current.AddRadicalApplication<TShellView>(configuration =>
            {
                configuration.DisableAutoBoot();
                configuration.OnServiceProviderCreated(serviceProvider => container = serviceProvider);
                if (enableSplashScreen)
                {
                    configuration.EnableSplashScreen(new SplashScreenConfiguration()
                    {
                        SplashScreenViewType = typeof(SingleViewSplashScreenView),
                        MinimumDelay = 0,
                        StartupAsyncWork = _ => Task.CompletedTask
                    });
                }
            });

            return (radicalApplication, () => container);
        }

        [AvaloniaTestMethod]
        public void Single_view_lifetime_should_show_the_shell_as_main_view()
        {
            UseSingleViewLifetime();
            var (radicalApplication, container) = CreateApplication<ShellControlView>();

            radicalApplication.Boot();

            Assert.AreSame(container().GetRequiredService<ShellControlView>(), mainView);
        }

        [AvaloniaTestMethod]
        public void Single_view_lifetime_should_replace_the_splash_screen_with_the_shell()
        {
            UseSingleViewLifetime();
            var (radicalApplication, container) = CreateApplication<ShellControlView>(enableSplashScreen: true);

            radicalApplication.Boot();
            Assert.AreSame(SingleViewSplashScreenView.LastCreated, mainView);

            var shell = container().GetRequiredService<ShellControlView>();
            Assert.IsTrue(UIThread.RunJobsUntil(() => mainView == shell), "The shell did not replace the splash screen.");
        }

        [AvaloniaTestMethod]
        public void Single_view_lifetime_should_not_support_window_shells()
        {
            UseSingleViewLifetime();
            var (radicalApplication, _) = CreateApplication<MainView>();

            var ex = Assert.ThrowsExactly<NotSupportedException>(() => radicalApplication.Boot());
            StringAssert.Contains(ex.Message, typeof(MainView).FullName);
        }

        [AvaloniaTestMethod]
        public void Activity_lifetime_should_create_a_main_view_showing_the_splash_screen_and_then_the_shell()
        {
            UseActivityLifetime();
            var (radicalApplication, container) = CreateApplication<ShellControlView>(enableSplashScreen: true);

            radicalApplication.Boot();
            var mainView = (ContentControl)mainViewFactory();
            Assert.AreSame(SingleViewSplashScreenView.LastCreated, mainView.Content);

            var shell = container().GetRequiredService<ShellControlView>();
            Assert.IsTrue(UIThread.RunJobsUntil(() => mainView.Content == shell), "The shell did not replace the splash screen.");
            Assert.AreSame(mainView, mainViewFactory());
        }

        [AvaloniaTestMethod]
        public void Control_shell_should_be_hosted_in_a_window_on_desktop()
        {
            var (radicalApplication, container) = CreateApplication<ShellControlView>();

            radicalApplication.Boot();

            var shell = container().GetRequiredService<ShellControlView>();
            var window = TopLevel.GetTopLevel(shell) as Window;
            try
            {
                Assert.IsNotNull(window);
                Assert.IsTrue(window.IsVisible);
            }
            finally
            {
                window?.Close();
            }
        }

        [AvaloniaTestMethod]
        public void Control_splash_screen_should_be_hosted_in_a_configured_window_on_desktop()
        {
            var (radicalApplication, container) = CreateApplication<MainView>(enableSplashScreen: true);

            radicalApplication.Boot();

            var splashWindow = TopLevel.GetTopLevel(SingleViewSplashScreenView.LastCreated) as Window;
            var shell = container().GetRequiredService<MainView>();
            try
            {
                Assert.IsNotNull(splashWindow);
                Assert.AreEqual(WindowDecorations.None, splashWindow.WindowDecorations);
                Assert.IsTrue(UIThread.RunJobsUntil(() => shell.IsVisible && !splashWindow.IsVisible), "The shell was not shown or the splash screen was not closed.");
            }
            finally
            {
                shell.Close();
                splashWindow?.Close();
            }
        }
    }
}
