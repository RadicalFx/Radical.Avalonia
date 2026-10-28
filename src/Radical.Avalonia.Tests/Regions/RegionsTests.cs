using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Avalonia.ComponentModel;
using Radical.Avalonia.Regions;
using Radical.Avalonia.Tests.Regions.Presentation;
using System;
using System.Linq;

namespace Radical.Avalonia.Tests.Regions
{
    [TestClass]
    public class RegionsTests
    {
        static IServiceProvider Boot()
        {
            IServiceProvider container = null;
            var radicalApplication = Application.Current.AddRadicalApplication(configuration =>
            {
                configuration.DisableAutoBoot();
                configuration.OnServiceProviderCreated(serviceProvider => container = serviceProvider);
            });
            radicalApplication.Boot();

            return container;
        }

        [AvaloniaTestMethod, TestCategory("Regions")]
        public void Regions_declared_in_xaml_should_be_registered_in_the_hosting_view_region_manager()
        {
            var container = Boot();
            var regionService = container.GetRequiredService<IRegionService>();

            var view = new RegionsHostView();

            Assert.IsTrue(regionService.HoldsRegionManager(view));
            var regions = regionService.GetRegionManager(view).GetAllRegisteredRegions().ToList();
            CollectionAssert.AreEquivalent(
                new[] { "TestContentRegion", "TestPanelRegion", "TestTabControlRegion" },
                regions.Select(r => r.Name).ToList());
            Assert.IsTrue(regions.All(r => r.HostingView == view));
        }

        [AvaloniaTestMethod, TestCategory("Regions")]
        public void Elements_region_should_add_and_remove_views()
        {
            var container = Boot();
            var view = new RegionsHostView();
            var manager = container.GetRequiredService<IRegionService>().GetRegionManager(view);
            var region = manager.GetRegion<IElementsRegion>("TestPanelRegion");
            var element = new Border();

            region.Add(element);
            var panel = view.FindControl<StackPanel>("PanelHost");
            CollectionAssert.Contains(panel.Children.ToList(), element);

            region.Remove(element);
            CollectionAssert.DoesNotContain(panel.Children.ToList(), element);
        }

        [AvaloniaTestMethod, TestCategory("Regions")]
        public void Views_should_be_injected_in_regions_when_the_hosting_view_is_loaded()
        {
            var container = Boot();
            var view = container.GetRequiredService<IViewResolver>().GetView<RegionsHostView>();
            var presenter = view.FindControl<ContentPresenter>("ContentHost");

            view.Show();
            try
            {
                var injected = UIThread.RunJobsUntil(() => presenter.Content is InjectedView);

                Assert.IsTrue(injected, "InjectedView was not injected in the region.");
            }
            finally
            {
                view.Close();
            }
        }
    }
}
