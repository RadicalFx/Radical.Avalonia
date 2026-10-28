using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.ComponentModel.Messaging;
using Radical.Avalonia.Bootstrap;
using Radical.Avalonia.ComponentModel;
using Radical.Avalonia.Services;

namespace Radical.Avalonia.Tests.Conventions
{
    [TestClass]
    public class GenerateViewModelStaticResourceKeyTests
    {
        [TestMethod, TestCategory("Conventions")]
        public void GenerateViewModelStaticResourceKey_should_generate_expected_key()
        {
            var obj = new TestTypeHelper();
            var expected = obj.GetType().Name;
            var conventions = new ConventionsHandler(A.Fake<IMessageBroker>(), A.Fake<IReleaseComponents>(), new BootstrapConventions());
            var key = conventions.GenerateViewModelStaticResourceKey(obj);

            Assert.AreEqual(expected, key);
        }

        [TestMethod, TestCategory("Conventions")]
        public void DefaultGenerateViewModelStaticResourceKey_should_generate_expected_key() 
        {
            var obj = new TestTypeHelper();
            var expected = obj.GetType().Name;
            var conventions = new ConventionsHandler(A.Fake<IMessageBroker>(), A.Fake<IReleaseComponents>(), new BootstrapConventions());
            var key = conventions.DefaultGenerateViewModelStaticResourceKey(obj);

            Assert.AreEqual(expected, key);
        }
    }
}