using Radical.Avalonia.ComponentModel;
using Radical.Avalonia.Regions;
using System;

namespace Radical.Avalonia.Bootstrap.Features
{
    class UIComposition : IFeature
    {
        public void Setup(IServiceProvider serviceProvider, BootstrapConfiguration bootstrapConfiguration)
        {
            RegionService.CurrentService = serviceProvider.GetService<IRegionService>();
            RegionService.Conventions = serviceProvider.GetService<IConventionsHandler>();
        }
    }
}
