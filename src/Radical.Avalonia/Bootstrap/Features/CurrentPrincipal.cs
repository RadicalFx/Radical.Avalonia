using System;
using System.Security.Principal;
using System.Threading;

namespace Radical.Avalonia.Bootstrap.Features
{
    class CurrentPrincipal : IFeature
    {
        public void Setup(IServiceProvider serviceProvider, BootstrapConfiguration bootstrapConfiguration)
        {
            if (OperatingSystem.IsWindows())
            {
                Thread.CurrentPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            }
        }
    }
}
