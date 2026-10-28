using Microsoft.Extensions.DependencyInjection;
using Radical.Avalonia.Bootstrap.Features;
using Radical.Avalonia.ComponentModel;
using Radical.Avalonia.Regions;
using System;
using System.Collections.Generic;

namespace Radical.Avalonia.Bootstrap.Installers
{
    class UICompositionInstaller : IDependenciesInstaller
    {
        public void Install(BootstrapConventions conventions, IServiceCollection services, IEnumerable<Type> assemblyScanningResults)
        {
            services.AddSingleton<IFeature, UIComposition>();
            services.AddSingleton<IRegionManagerFactory, RegionManagerFactory>();
            services.AddSingleton<IRegionService, RegionService>();
            services.AddTransient<IRegionManager, RegionManager>();
        }
    }
}
