using Avalonia.Controls;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Avalonia.Bootstrap;
using Radical.Avalonia.ComponentModel;
using Radical.Avalonia.Services;
using Radical.ComponentModel.Messaging;

namespace Radical.Avalonia.Tests.Conventions
{
    [TestClass]
    public class FindWindowOfTests
    {
        static ConventionsHandler CreateConventions() =>
            new ConventionsHandler(A.Fake<IMessageBroker>(), A.Fake<IReleaseComponents>(), new BootstrapConventions());

        [AvaloniaTestMethod, TestCategory("Conventions")]
        public void FindWindowOf_should_return_the_hosting_window()
        {
            var conventions = CreateConventions();
            var view = new Border();
            var window = new Window { Content = view };

            var found = conventions.FindWindowOf(view);

            Assert.AreSame(window, found);
        }

        [AvaloniaTestMethod, TestCategory("Conventions")]
        public void FindWindowOf_should_return_null_for_detached_views()
        {
            var conventions = CreateConventions();

            var found = conventions.FindWindowOf(new Border());

            Assert.IsNull(found);
        }
    }
}
