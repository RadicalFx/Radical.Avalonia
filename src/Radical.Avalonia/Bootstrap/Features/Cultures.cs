using System;
using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;

namespace Radical.Avalonia.Bootstrap.Features
{
    class Cultures : IFeature
    {
        public void Setup(IServiceProvider serviceProvider, BootstrapConfiguration bootstrapConfiguration)
        {
            var currentCulture = bootstrapConfiguration.CultureConfig(serviceProvider);
            var currentUICulture = bootstrapConfiguration.UICultureConfig(serviceProvider);

            Thread.CurrentThread.CurrentCulture = currentCulture;
            Thread.CurrentThread.CurrentUICulture = currentUICulture;

            if (UnitTestDetector.IsTest) 
            {
                return;
            }

            var fd = currentUICulture.TextInfo.IsRightToLeft ?
                FlowDirection.RightToLeft :
                FlowDirection.LeftToRight;

            // Avalonia has no metadata override for inherited properties: the flow
            // direction is applied, with style priority so that local values win,
            // to every top-level window when it opens and is inherited from there.
            Window.WindowOpenedEvent.AddClassHandler<Window>((window, _) =>
            {
                window.SetValue(Visual.FlowDirectionProperty, fd, BindingPriority.Style);
            });
        }
    }

    /// <summary>
    /// Detect if we are running as part of a unit test.
    /// This is DIRTY and should only be used if absolutely necessary 
    /// as its usually a sign of bad design.
    /// </summary>    
    static class UnitTestDetector
    {
        static UnitTestDetector()
        {
            IsTest = AppDomain.CurrentDomain.GetAssemblies().Any(assembly =>
            {
                return assembly.FullName.StartsWith("Microsoft.VisualStudio.TestPlatform", StringComparison.InvariantCultureIgnoreCase);
            });
        }

        public static bool IsTest { get; }
    }
}
