using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Avalonia.Behaviors;
using Radical.Avalonia.Bootstrap;
using Radical.Avalonia.ComponentModel;
using Radical.Avalonia.Services;
using Radical.ComponentModel.Messaging;
using System.Linq;

namespace Radical.Avalonia.Tests.Conventions
{
    [TestClass]
    public class DetachViewBehaviorsTests
    {
        class UserBehavior : Behavior<Control> { }

        static ConventionsHandler CreateConventions() =>
            new ConventionsHandler(A.Fake<IMessageBroker>(), A.Fake<IReleaseComponents>(), new BootstrapConventions());

        [AvaloniaTestMethod, TestCategory("Conventions")]
        public void DetachViewBehaviors_should_remove_only_radical_behaviors_from_attached_views()
        {
            var conventions = CreateConventions();
            var view = new UserControl();
            conventions.AttachViewBehaviors(view);
            var userBehavior = new UserBehavior();
            Interaction.GetBehaviors(view).Add(userBehavior);
            var window = new Window { Content = view };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            conventions.DetachViewBehaviors(view);

            CollectionAssert.AreEqual(new[] { userBehavior }, Interaction.GetBehaviors(view).ToList());
            Assert.AreSame(view, userBehavior.AssociatedObject);
            window.Close();
        }

        [AvaloniaTestMethod, TestCategory("Conventions")]
        public void DetachViewBehaviors_should_not_throw_for_views_whose_window_was_closed()
        {
            var conventions = CreateConventions();
            var view = new UserControl();
            conventions.AttachViewBehaviors(view);
            var userBehavior = new UserBehavior();
            Interaction.GetBehaviors(view).Add(userBehavior);
            var window = new Window { Content = view };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.Close();
            Dispatcher.UIThread.RunJobs();

            conventions.DetachViewBehaviors(view);

            CollectionAssert.AreEqual(new[] { userBehavior }, Interaction.GetBehaviors(view).ToList());
            Assert.IsFalse(Interaction.GetBehaviors(view).OfType<FrameworkElementLifecycleNotificationsBehavior>().Any());
        }
    }
}
