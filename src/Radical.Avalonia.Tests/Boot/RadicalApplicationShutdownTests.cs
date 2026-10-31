using Avalonia;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Avalonia.Bootstrap;
using Radical.Avalonia.Messaging;
using Radical.ComponentModel.Messaging;
using System;
using System.Threading.Tasks;

namespace Radical.Avalonia.Tests.Boot
{
    [TestClass]
    public class RadicalApplicationShutdownTests
    {
        class RecordingShellHost : ShellHost
        {
            public override void ShowSplashScreen(Control view, SplashScreenConfiguration configuration) { }

            public override void ShowShell(Control view) { }
        }

        Func<global::Avalonia.Controls.ApplicationLifetimes.IApplicationLifetime, ShellHost> previousFactory;

        [TestInitialize]
        public void Setup()
        {
            previousFactory = ShellHost.Factory;
            ShellHost.Factory = _ => new RecordingShellHost();
        }

        [TestCleanup]
        public void Cleanup()
        {
            ShellHost.Factory = previousFactory;
        }

        static RadicalApplication Boot(Func<object, ApplicationShutdownRequested, Task> onShutdownRequested, Action onShuttingDown)
        {
            var radicalApplication = Application.Current.AddRadicalApplication(configuration =>
            {
                configuration.DisableAutoBoot();
                configuration.OnBootCompleted(serviceProvider =>
                {
                    var broker = serviceProvider.GetRequiredService<IMessageBroker>();
                    broker.Subscribe(serviceProvider, onShutdownRequested);
                });
                configuration.OnShuttingDown(_ => onShuttingDown());
            });

            radicalApplication.Boot();

            return radicalApplication;
        }

        [AvaloniaTestMethod]
        public async Task ShutdownAsync_should_await_ApplicationShutdownRequested_subscribers_before_shutting_down()
        {
            var gate = new TaskCompletionSource<bool>();
            var shuttingDown = false;

            var radicalApplication = Boot(async (s, m) => await gate.Task, () => shuttingDown = true);

            var shutdown = radicalApplication.ShutdownAsync();

            Assert.IsFalse(shutdown.IsCompleted);
            Assert.IsFalse(shuttingDown);

            gate.SetResult(true);
            await shutdown;

            Assert.IsTrue(shuttingDown);
        }

        [AvaloniaTestMethod]
        public async Task ShutdownAsync_should_be_canceled_by_an_async_ApplicationShutdownRequested_subscriber()
        {
            var shuttingDown = false;

            var radicalApplication = Boot(async (s, m) =>
            {
                await Task.Yield();
                m.Cancel = true;
            }, () => shuttingDown = true);

            await radicalApplication.ShutdownAsync();

            Assert.IsFalse(shuttingDown);
        }
    }
}
