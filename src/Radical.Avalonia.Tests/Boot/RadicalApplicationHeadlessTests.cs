using Avalonia;
using Avalonia.Controls;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Avalonia.Bootstrap;
using System;
using System.Threading.Tasks;

namespace Radical.Avalonia.Tests.Boot
{
    [TestClass]
    public class RadicalApplicationHeadlessTests
    {
        /*
         * Applications without a shell, e.g. starting hidden in the
         * tray area, must boot without showing any view: the shell
         * host records what the boot process asks it to show.
         */
        class RecordingShellHost : ShellHost
        {
            public int ShownViews { get; private set; }

            public override void ShowSplashScreen(Control view, SplashScreenConfiguration configuration) => ShownViews++;

            public override void ShowShell(Control view) => ShownViews++;
        }

        RecordingShellHost shellHost;
        Func<global::Avalonia.Controls.ApplicationLifetimes.IApplicationLifetime, ShellHost> previousFactory;

        [TestInitialize]
        public void Setup()
        {
            previousFactory = ShellHost.Factory;
            shellHost = new RecordingShellHost();
            ShellHost.Factory = _ => shellHost;
        }

        [TestCleanup]
        public void Cleanup()
        {
            ShellHost.Factory = previousFactory;
        }

        [AvaloniaTestMethod]
        public void Application_without_a_shell_boots_without_showing_views()
        {
            var bootCompleted = false;

            var radicalApplication = Application.Current.AddRadicalApplication(configuration =>
            {
                configuration.DisableAutoBoot();
                configuration.OnBootCompleted(_ => bootCompleted = true);
            });

            radicalApplication.Boot();

            Assert.IsTrue(bootCompleted);
            Assert.AreEqual(0, shellHost.ShownViews);
        }

        [AvaloniaTestMethod]
        public async Task Application_without_a_shell_using_host_builder_boots_without_showing_views()
        {
            var bootCompleted = false;

            var host = new HostBuilder()
                .AddRadicalApplication(configuration =>
                {
                    configuration.OnBootCompleted(_ => bootCompleted = true);
                })
                .Build();

            await host.StartAsync();

            Assert.IsTrue(bootCompleted);
            Assert.AreEqual(0, shellHost.ShownViews);

            using (host)
            {
                await host.StopAsync();
            }
        }
    }
}
