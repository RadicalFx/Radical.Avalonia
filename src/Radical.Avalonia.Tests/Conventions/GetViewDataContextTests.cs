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
    public class GetViewDataContextTests
    {
        static ConventionsHandler CreateConventions() =>
            new ConventionsHandler(A.Fake<IMessageBroker>(), A.Fake<IReleaseComponents>(), new BootstrapConventions());

        [AvaloniaTestMethod, TestCategory("Conventions")]
        public void LocalOnly_search_should_ignore_inherited_data_context()
        {
            var conventions = CreateConventions();
            var child = new Border();
            var parent = new Border { Child = child, DataContext = new object() };

            var dc = conventions.GetViewDataContext(child, ViewDataContextSearchBehavior.LocalOnly);

            Assert.IsNull(dc);
        }

        [AvaloniaTestMethod, TestCategory("Conventions")]
        public void LocalOnly_search_should_return_locally_set_data_context()
        {
            var conventions = CreateConventions();
            var expected = new object();
            var view = new Border { DataContext = expected };

            var dc = conventions.GetViewDataContext(view, ViewDataContextSearchBehavior.LocalOnly);

            Assert.AreSame(expected, dc);
        }

        [AvaloniaTestMethod, TestCategory("Conventions")]
        public void Legacy_search_should_return_inherited_data_context()
        {
            var conventions = CreateConventions();
            var expected = new object();
            var child = new Border();
            var parent = new Border { Child = child, DataContext = expected };

            var dc = conventions.GetViewDataContext(child, ViewDataContextSearchBehavior.Legacy);

            Assert.AreSame(expected, dc);
        }

        [AvaloniaTestMethod, TestCategory("Conventions")]
        public void SetViewDataContext_should_set_the_data_context()
        {
            var conventions = CreateConventions();
            var expected = new object();
            var view = new Border();

            conventions.SetViewDataContext(view, expected);

            Assert.AreSame(expected, view.DataContext);
        }
    }
}
