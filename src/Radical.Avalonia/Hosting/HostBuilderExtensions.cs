using Microsoft.Extensions.DependencyInjection;
using Radical.Avalonia.Bootstrap;
using Radical.Avalonia.Hosting;
using System;
using Avalonia;
using Avalonia.Controls;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// Extension methods to add a Radical application to an <see cref="IHostBuilder"/>.
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Add a RadicalApplication to the current host builder
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="configure">A delegate to customize the bootstrap configuration, or <c>null</c>.</param>
        /// <returns>The host builder, for chaining.</returns>
        public static IHostBuilder AddRadicalApplication(this IHostBuilder hostBuilder, Action<BootstrapConfiguration> configure)
        {
            return AddRadicalApplication(hostBuilder, null, configure);
        }

        /// <summary>
        /// Add a RadicalApplication to the current host builder
        /// </summary>
        /// <typeparam name="TShellView">The type of the shell view.</typeparam>
        /// <param name="hostBuilder">The host builder.</param>
        /// <returns>The host builder, for chaining.</returns>
        public static IHostBuilder AddRadicalApplication<TShellView>(this IHostBuilder hostBuilder) where TShellView : Control
        {
            return AddRadicalApplication(hostBuilder, typeof(TShellView), null);
        }

        /// <summary>
        /// Add a RadicalApplication to the current host builder
        /// </summary>
        /// <typeparam name="TShellView">The type of the shell view.</typeparam>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="configure">A delegate to customize the bootstrap configuration, or <c>null</c>.</param>
        /// <returns>The host builder, for chaining.</returns>
        public static IHostBuilder AddRadicalApplication<TShellView>(this IHostBuilder hostBuilder, Action<BootstrapConfiguration> configure) where TShellView : Control
        {
            return AddRadicalApplication(hostBuilder, typeof(TShellView), configure);
        }

        /// <summary>
        /// Add a RadicalApplication to the current host builder
        /// </summary>
        /// <param name="configure">A delegate to customize the bootstrap configuration, or <c>null</c>.</param>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="shellType">The type of the shell view, or <c>null</c> to use the default one.</param>
        /// <returns>The host builder, for chaining.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>")]
        public static IHostBuilder AddRadicalApplication(this IHostBuilder hostBuilder, Type shellType, Action<BootstrapConfiguration> configure)
        {
            if (hostBuilder.Properties.TryGetValue("RadicalApplicationAdded", out object val))
            {
                throw new InvalidOperationException("A Radical application has been already added to the current host.");
            }

            var configuration = new BootstrapConfiguration();
            if (shellType != null)
            {
                configuration.UseAsShell(shellType);
            }

            configure?.Invoke(configuration);

            hostBuilder.ConfigureServices((context, serviceCollection) =>
            {
                configuration.ConfigureServicesHandler(serviceCollection);
                configuration.PopulateServiceCollection(serviceCollection);

                serviceCollection.AddSingleton(new LifetimeExitTracker());
                serviceCollection.AddHostedService<RadicalApplicationService>();
            });

            hostBuilder.Properties.Add("RadicalApplicationAdded", new object());

            return hostBuilder;
        }
    }
}