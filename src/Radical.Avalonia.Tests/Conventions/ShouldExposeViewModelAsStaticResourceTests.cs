using Avalonia;
using Avalonia.Controls;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.ComponentModel.Messaging;
using Radical.Avalonia.Bootstrap;
using Radical.Avalonia.ComponentModel;
using Radical.Avalonia.Services;

namespace Radical.Avalonia.Tests.Conventions
{
    [TestClass]
    public class ShouldExposeViewModelAsStaticResourceTests
    {
        class ViewModelNoAttribute
        {
        }

        [ExposeViewModelAsStaticResource]
        class ViewModelWithAttribute
        {
        }

        class NopView : AvaloniaObject
        {
        }

        [TestMethod, TestCategory("Conventions")]
        public void ShouldExposeViewModelAsStaticResource_should_return_false_if_no_attribute()
        {
            var conventions = new ConventionsHandler(A.Fake<IMessageBroker>(), A.Fake<IReleaseComponents>(), new BootstrapConventions());
            var key = conventions.ShouldExposeViewModelAsStaticResource(new NopView(), new ViewModelNoAttribute());

            Assert.AreEqual(false, key);
        }

        [TestMethod, TestCategory("Conventions")]
        public void ShouldExposeViewModelAsStaticResource_should_return_false_if_attribute()
        {
            var conventions = new ConventionsHandler(A.Fake<IMessageBroker>(), A.Fake<IReleaseComponents>(), new BootstrapConventions());
            var key = conventions.ShouldExposeViewModelAsStaticResource(new NopView(), new ViewModelWithAttribute());

            Assert.AreEqual(true, key);
        }
    }
}
