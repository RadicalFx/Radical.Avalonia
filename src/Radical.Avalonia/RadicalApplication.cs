using Microsoft.Extensions.DependencyInjection;
using Radical.ComponentModel.Messaging;
using Radical.Linq;
using Radical.Validation;
using Radical.Avalonia.Bootstrap;
using Radical.Avalonia.ComponentModel;
using Radical.Avalonia.Messaging;
using Radical.Avalonia.Regions;
using System;
using System.Linq;
using System.Threading;
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
                if (application.ApplicationLifetime is IControlledApplicationLifetime lifetime)
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

            if (application.ApplicationLifetime is IControlledApplicationLifetime lifetime)
            {
                lifetime.Exit += (s, e) =>
                {
                    if (!isShuttingDown)
                    {
                        OnShutdownCore(ApplicationShutdownReason.ApplicationRequest);
                    }
                };
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

            if (bootstrapConfiguration.ShutdownMode != null && application.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
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
                        (application.ApplicationLifetime as IControlledApplicationLifetime)?.Shutdown();
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

        void OnBootCompleted(IServiceProvider serviceProvider)
        {
            if (bootstrapConfiguration.ShellViewType != null)
            {
                var resolver = serviceProvider.GetService<IViewResolver>();
                var mainView = (Window)resolver.GetView(bootstrapConfiguration.ShellViewType);
                if (application.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    desktop.MainWindow = mainView;
                }

                mainView.Show();
            }
        }
    }
}