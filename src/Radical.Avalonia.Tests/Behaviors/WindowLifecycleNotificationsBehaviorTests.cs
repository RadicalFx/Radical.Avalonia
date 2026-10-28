using Avalonia.Controls;
using Avalonia.Threading;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Avalonia.Bootstrap;
using Radical.Avalonia.ComponentModel;
using Radical.Avalonia.Regions;
using Radical.Avalonia.Services;
using Radical.ComponentModel.Messaging;
using System.Collections.Generic;
using System.ComponentModel;

namespace Radical.Avalonia.Tests.Behaviors
{
    [TestClass]
    public class WindowLifecycleNotificationsBehaviorTests
    {
        class LifecycleViewModel : IExpectViewLoadedCallback, IExpectViewShownCallback, IExpectViewClosingCallback, IExpectViewClosedCallback
        {
            public List<string> Calls { get; } = new List<string>();

            public void OnViewLoaded() => Calls.Add("Loaded");
            public void OnViewShown() => Calls.Add("Shown");
            public void OnViewClosing(CancelEventArgs e) => Calls.Add("Closing");
            public void OnViewClosed() => Calls.Add("Closed");
        }

        IRegionService previousRegionService;

        [TestInitialize]
        public void Setup()
        {
            previousRegionService = RegionService.CurrentService;
            RegionService.CurrentService = A.Fake<IRegionService>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            RegionService.CurrentService = previousRegionService;
        }

        static (Window, LifecycleViewModel) CreateView()
        {
            var conventions = new ConventionsHandler(A.Fake<IMessageBroker>(), A.Fake<IReleaseComponents>(), new BootstrapConventions());
            var viewModel = new LifecycleViewModel();
            var window = new Window();
            conventions.SetViewDataContext(window, viewModel);
            conventions.AttachViewBehaviors(window);

            return (window, viewModel);
        }

        [AvaloniaTestMethod]
        public void Window_lifecycle_callbacks_should_not_be_invoked_before_the_window_is_shown()
        {
            var (_, viewModel) = CreateView();

            Dispatcher.UIThread.RunJobs();

            CollectionAssert.AreEqual(new string[0], viewModel.Calls);
        }

        [AvaloniaTestMethod]
        public void Window_lifecycle_callbacks_should_be_invoked_in_order()
        {
            var (window, viewModel) = CreateView();

            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.Close();
            Dispatcher.UIThread.RunJobs();

            CollectionAssert.AreEqual(new[] { "Loaded", "Shown", "Closing", "Closed" }, viewModel.Calls);
        }
    }
}
