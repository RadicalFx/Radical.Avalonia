using Microsoft.Extensions.DependencyInjection;
using Radical.ComponentModel;
using Radical.Avalonia.Bootstrap.Features;
using Radical.Avalonia.ComponentModel;
using Radical.Avalonia.Threading;
using System;
using System.Collections.Generic;
using Avalonia;
using AvaloniaThreading = Avalonia.Threading;

namespace Radical.Avalonia.Bootstrap.Installers
{
    class DefaultInstaller : IDependenciesInstaller
    {
        public void Install(BootstrapConventions conventions, IServiceCollection services, IEnumerable<Type> assemblyScanningResults)
        {
            services.AddSingleton<IFeature, Cultures>();
            services.AddSingleton<IFeature, CurrentPrincipal>();

            services.AddSingleton(container => Application.Current);
            services.AddSingleton(container => AvaloniaThreading.Dispatcher.UIThread);

            services.AddSingleton<IDispatcher, AvaloniaDispatcher>();
            services.AddSingleton<IReleaseComponents, DefaultComponentReleaser>();
        }
    }
}
