using Microsoft.Extensions.DependencyInjection;
using Radical.ComponentModel.Messaging;
using Radical.Linq;
using Radical.Validation;
using Radical.Avalonia.Bootstrap;
using Radical.Avalonia.ComponentModel;
using Radical.Avalonia.Messaging;
using Radical.Avalonia.Regions;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;

namespace Radical.Avalonia
{
    /// <summary>
    /// The Radical application.
    /// </summary>
    public sealed class RadicalApplication
    {
        /// <summary>
        /// Gets the lifetime of the given application, replaceable by tests
        /// as the lifetime cannot be changed once the application is initialized.
        /// </summary>
        internal static Func<Application, IApplicationLifetime> LifetimeOf = application => application.ApplicationLifetime;

        /// <summary>
        /// Create a Radical application with a life-cycle
        /// bound to the supplied Avalonia application, using
        /// the given configuration.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>")]
        internal static RadicalApplication BoundTo(Application application, BootstrapConfiguration bootstrapConfiguration)
        {
            Ensure.That(bootstrapConfiguration)
                .Named(nameof(bootstrapConfiguration))
                .IsNotNull();

            var services = new ServiceCollection();
            bootstrapConfiguration.ConfigureServicesHandler(services);
            bootstrapConfiguration.PopulateServiceCollection(services);
            var serviceProvider = services.BuildServiceProvider();
            bootstrapConfiguration.OnServiceProviderCreatedHandler(serviceProvider);

            var radicalApplication = new RadicalApplication(application, serviceProvider, bootstrapConfiguration);

            if (bootstrapConfiguration.IsAutoBootEnabled)
            {
                if (LifetimeOf(application) is IControlledApplicationLifetime lifetime)
                {
                    lifetime.Startup += (s, e) => radicalApplication.BootApplication();
                }
                else
                {
                    // No controlled lifetime (e.g. headless or single-view platforms):
                    // there is no Startup event to wait for, boot right away.
                    radicalApplication.BootApplication();
                }
            }

            return radicalApplication;
        }

        internal static RadicalApplication ExternallyManagedBy(Application application, IServiceProvider serviceProvider, BootstrapConfiguration bootstrapConfiguration)
        {
            Ensure.That(bootstrapConfiguration.IsAutoBootEnabled)
                .WithMessage("When using Generic Host auto boot cannot be disabled.")
                .Is(true);

            return new RadicalApplication(application, serviceProvider, bootstrapConfiguration);
        }

        readonly Application application;
        readonly BootstrapConfiguration bootstrapConfiguration;
        readonly IServiceProvider serviceProvider;
        bool isShuttingDown = false;
        bool isBootCompleted = false;
        Mutex singletonApplicationMutex;

        internal RadicalApplication(Application application, IServiceProvider serviceProvider, BootstrapConfiguration bootstrapConfiguration)
        {
            this.application = application;
            this.serviceProvider = serviceProvider;
            this.bootstrapConfiguration = bootstrapConfiguration;

            Dispatcher.UIThread.UnhandledException += (s, e) =>
            {
                var ex = e.Exception;
                bootstrapConfiguration.UnhandledExceptionHandler(ex);
            };

            if (LifetimeOf(application) is IControlledApplicationLifetime lifetime)
            {
                lifetime.Exit += (s, e) => OnLifetimeExit();
            }

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                bootstrapConfiguration.UnhandledExceptionHandler(ex);
            };
        }

        internal void BootApplication()
        {
            var features = serviceProvider.GetServices<IFeature>();
            foreach (var feature in features)
            {
                feature.Setup(serviceProvider, bootstrapConfiguration);
            }

            if (bootstrapConfiguration.ShutdownMode != null && LifetimeOf(application) is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.ShutdownMode = bootstrapConfiguration.ShutdownMode.Value;
            }

            var broker = serviceProvider.GetService<IMessageBroker>();
            broker.Subscribe<ApplicationShutdownRequest>(this, InvocationModel.Safe, (s, m) =>
            {
                OnShutdownCore(ApplicationShutdownReason.UserRequest);
            });

            var args = new SingletonApplicationStartupArgs(bootstrapConfiguration.SingletonApplicationScope, bootstrapConfiguration.SingletonRegistrationKey);
            HandleSingletonApplicationStartup(args);

            if (args.AllowStartup)
            {
                bootstrapConfiguration.BootingHandler(serviceProvider);
            }
            else
            {
                OnShutdownCore(ApplicationShutdownReason.MultipleInstanceNotAllowed);
            }

            if (!isShuttingDown)
            {
                OnBootCompleted(serviceProvider);

                bootstrapConfiguration.ExposeRegisteredAppResources(serviceProvider, application);

                broker.Broadcast(this, new ApplicationBootCompleted());

                bootstrapConfiguration.BootCompletedHandler(serviceProvider);

                var callbacks = serviceProvider.GetServices<IExpectBootCallback>();
                if (callbacks != null && callbacks.Any())
                {
                    foreach (var cb in callbacks)
                    {
                        cb.OnBootCompleted();
                    }
                }

                isBootCompleted = true;
            }
        }

        void HandleSingletonApplicationStartup(SingletonApplicationStartupArgs args)
        {
            if (args.Scope != SingletonApplicationScope.NotSupported)
            {
                string mutexName = @"Local\" + args.SingletonRegistrationKey;
                if (args.Scope == SingletonApplicationScope.Global)
                {
                    mutexName = @"Global\" + args.SingletonRegistrationKey;
                }

                singletonApplicationMutex = new Mutex(false, mutexName);
                args.AllowStartup = singletonApplicationMutex.WaitOne(TimeSpan.Zero, false);

                bootstrapConfiguration.OnSingletonApplicationStartupHandler(args);
            }
        }

        void OnShutdownCore(ApplicationShutdownReason reason)
        {
            if (isShuttingDown)
            {
                // e.g. the lifetime raising Exit while shutting down
                return;
            }

            var canceled = false;

            try
            {
                if (reason == ApplicationShutdownReason.UserRequest && isBootCompleted)
                {
                    var msg = new ApplicationShutdownRequested(reason);

                    var broker = serviceProvider.GetService<IMessageBroker>();
                    broker.Dispatch(this, msg);

                    canceled = msg.Cancel;

                    if (canceled)
                    {
                        broker.Broadcast(this, new ApplicationShutdownCanceled(reason));
                        return;
                    }
                }

                isShuttingDown = true;

                if (isBootCompleted)
                {
                    serviceProvider
                        .GetService<IMessageBroker>()
                        .Broadcast(this, new ApplicationShutdown(reason));

                    var callbacks = serviceProvider.GetServices<IExpectShutdownCallback>();
                    if (callbacks != null && callbacks.Any())
                    {
                        foreach (var cb in callbacks)
                        {
                            cb.OnShutdown(reason);
                        }
                    }
                }

                bootstrapConfiguration.ShuttingDownHandler(new ApplicationShuttingDownArgs()
                {
                    Reason = reason,
                    IsBootCompleted = isBootCompleted
                });
            }
            finally
            {
                if (!canceled)
                {
                    if (reason != ApplicationShutdownReason.ApplicationRequest)
                    {
                        (LifetimeOf(application) as IControlledApplicationLifetime)?.Shutdown();
                    }

                    singletonApplicationMutex?.Dispose();
                    singletonApplicationMutex = null;

                    RegionService.CurrentService = null;
                    RegionService.Conventions = null;
                }
            }
        }

        /// <summary>
        /// Boots this instance.
        /// </summary>
        public void Boot()
        {
            if (!bootstrapConfiguration.IsAutoBootEnabled && !isBootCompleted)
            {
                BootApplication();
            }
        }

        /// <summary>
        /// Shutdowns this application.
        /// </summary>
        public void Shutdown()
        {
            OnShutdownCore(ApplicationShutdownReason.UserRequest);
        }

        /// <summary>
        /// Shuts down this application because the application lifetime is exiting,
        /// without shutting down the lifetime again.
        /// </summary>
        internal void OnLifetimeExit()
        {
            OnShutdownCore(ApplicationShutdownReason.ApplicationRequest);
        }

        void OnBootCompleted(IServiceProvider serviceProvider)
        {
            var resolver = serviceProvider.GetService<IViewResolver>();
            var shellHost = ShellHost.Factory(LifetimeOf(application));

            void showSplash()
            {
                var splashScreenConfiguration = bootstrapConfiguration.SplashScreenConfiguration;
                var splashScreen = (Control)resolver.GetView(splashScreenConfiguration.SplashScreenViewType);

                shellHost.ShowSplashScreen(splashScreen, splashScreenConfiguration);
            }

            void showShell()
            {
                if (bootstrapConfiguration.ShellViewType != null)
                {
                    var mainView = (Control)resolver.GetView(bootstrapConfiguration.ShellViewType);
                    shellHost.ShowShell(mainView);
                }
            }

            if (bootstrapConfiguration.IsSplashScreenEnabled)
            {
                showSplash();

                async Task action()
                {
                    var sw = Stopwatch.StartNew();

                    await bootstrapConfiguration.SplashScreenConfiguration.StartupAsyncWork(serviceProvider)
                        .ConfigureAwait(true);

                    sw.Stop();
                    var elapsed = (int)sw.ElapsedMilliseconds;
                    var remaining = bootstrapConfiguration.SplashScreenConfiguration.MinimumDelay - elapsed;
                    if (remaining > 0)
                    {
                        await Task.Delay(remaining)
                            .ConfigureAwait(true);
                    }
                }

                var startup = Task.Run(action);

                startup.ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        bootstrapConfiguration.UnhandledExceptionHandler(t.Exception);
                        throw t.Exception;
                    }

                    showShell();
                }, Dispatcher.UIThread.ToTaskScheduler());
            }
            else
            {
                showShell();
            }
        }
    }
}