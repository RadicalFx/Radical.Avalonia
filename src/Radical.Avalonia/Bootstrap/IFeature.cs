using System;

namespace Radical.Avalonia.Bootstrap
{
    interface IFeature
    {
        void Setup(IServiceProvider serviceProvider, BootstrapConfiguration bootstrapConfiguration);
    }
}
