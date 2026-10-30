using Radical.Avalonia;
using Avalonia.Controls;
using Radical.Avalonia.Bootstrap;
using System;

namespace Avalonia
{
    /// <summary>
    /// Extension methods to bootstrap a <see cref="RadicalApplication"/> from an Avalonia <see cref="Application"/>.
    /// </summary>
    public static class ApplicationExtensions
    {
        /// <summary>
        /// Add a RadicalApplication to the current Avalonia application
        /// </summary>
        /// <param name="application">The Avalonia application to bind the Radical application to.</param>
        /// <param name="configure">A delegate to customize the bootstrap configuration, or <c>null</c>.</param>
        /// <returns>The configured <see cref="RadicalApplication"/>.</returns>
        public static RadicalApplication AddRadicalApplication(this Application application, Action<BootstrapConfiguration> configure)
        {
            return AddRadicalApplication(application, null, configure);
        }

        /// <summary>
        /// Add a RadicalApplication to the current Avalonia application
        /// </summary>
        /// <typeparam name="TShellView">The type of the shell view.</typeparam>
        /// <param name="application">The Avalonia application to bind the Radical application to.</param>
        /// <returns>The configured <see cref="RadicalApplication"/>.</returns>
        public static RadicalApplication AddRadicalApplication<TShellView>(this Application application) where TShellView : Control
        {
            return AddRadicalApplication(application, typeof(TShellView), null);
        }

        /// <summary>
        /// Add a RadicalApplication to the current Avalonia application
        /// </summary>
        /// <typeparam name="TShellView">The type of the shell view.</typeparam>
        /// <param name="application">The Avalonia application to bind the Radical application to.</param>
        /// <param name="configure">A delegate to customize the bootstrap configuration, or <c>null</c>.</param>
        /// <returns>The configured <see cref="RadicalApplication"/>.</returns>
        public static RadicalApplication AddRadicalApplication<TShellView>(this Application application, Action<BootstrapConfiguration> configure) where TShellView : Control
        {
            return AddRadicalApplication(application, typeof(TShellView), configure);
        }

        /// <summary>
        /// Add a RadicalApplication to the current Avalonia application
        /// </summary>
        /// <param name="application">The Avalonia application to bind the Radical application to.</param>
        /// <param name="shellViewType">The type of the shell view, or <c>null</c> to use the default one.</param>
        /// <param name="configure">A delegate to customize the bootstrap configuration, or <c>null</c>.</param>
        /// <returns>The configured <see cref="RadicalApplication"/>.</returns>
        public static RadicalApplication AddRadicalApplication(this Application application, Type shellViewType, Action<BootstrapConfiguration> configure)
        {
            var configuration = new BootstrapConfiguration();
            if (shellViewType != null)
            {
                configuration.UseAsShell(shellViewType);
            }
            configure?.Invoke(configuration);

            return RadicalApplication.BoundTo(application, configuration);
        }
    }
}