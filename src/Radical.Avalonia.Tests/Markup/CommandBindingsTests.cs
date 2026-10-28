using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Avalonia.ComponentModel;
using System.Linq;

namespace Radical.Avalonia.Tests.Markup
{
    [TestClass]
    public class CommandBindingsTests
    {
        static (CommandsView, CommandsViewModel) CreateView()
        {
            var viewModel = new CommandsViewModel();
            var view = new CommandsView { DataContext = viewModel };

            return (view, viewModel);
        }

        [AvaloniaTestMethod, TestCategory("Markup")]
        public void AutoCommandBinding_should_generate_a_command_bound_to_the_data_context_method()
        {
            var (view, viewModel) = CreateView();
            var button = view.FindControl<Button>("AutoButton");

            Assert.IsInstanceOfType<IDelegateCommand>(button.Command);
            button.Command.Execute(button.CommandParameter);

            Assert.AreEqual(42, viewModel.SavedValue);
        }

        [AvaloniaTestMethod, TestCategory("Markup")]
        public void AutoCommandBinding_should_honor_the_Can_property()
        {
            var (view, viewModel) = CreateView();
            var button = view.FindControl<Button>("AutoButton");
            var canExecuteChanged = false;
            button.Command.CanExecuteChanged += (s, e) => canExecuteChanged = true;

            viewModel.CanSave = false;

            Assert.IsTrue(canExecuteChanged);
            Assert.IsFalse(button.Command.CanExecute(button.CommandParameter));
        }

        [AvaloniaTestMethod, TestCategory("Markup")]
        public void CommandBinding_should_bind_the_data_context_command()
        {
            var (view, viewModel) = CreateView();
            var button = view.FindControl<Button>("BoundButton");

            Assert.AreSame(viewModel.RefreshCommand, button.Command);
        }

        [AvaloniaTestMethod, TestCategory("Markup")]
        public void Command_key_bindings_should_be_registered_on_the_root_element_when_loaded()
        {
            var (view, viewModel) = CreateView();

            view.Show();
            try
            {
                Dispatcher.UIThread.RunJobs();

                var saveBinding = view.KeyBindings.SingleOrDefault(kb => kb.Gesture.Equals(new KeyGesture(Key.S, KeyModifiers.Control)));
                Assert.IsNotNull(saveBinding);
                Assert.AreEqual("42", saveBinding.CommandParameter);
                Assert.IsTrue(view.KeyBindings.Any(kb => kb.Command == viewModel.RefreshCommand && kb.Gesture.Equals(new KeyGesture(Key.F5))));

                saveBinding.Command.Execute(saveBinding.CommandParameter);
                Assert.AreEqual(42, viewModel.SavedValue);
            }
            finally
            {
                view.Close();
            }
        }

        [AvaloniaTestMethod, TestCategory("Markup")]
        public void EditorBinding_should_bind_two_way()
        {
            var (view, viewModel) = CreateView();
            var editor = view.FindControl<TextBox>("Editor");

            viewModel.Name = "from view model";
            Assert.AreEqual("from view model", editor.Text);

            editor.Text = "from view";
            Assert.AreEqual("from view", viewModel.Name);
        }

        [AvaloniaTestMethod, TestCategory("Markup")]
        public void Behavior_command_bindings_should_generate_commands_when_the_attached_object_is_loaded()
        {
            var (view, viewModel) = CreateView();
            var host = view.FindControl<Button>("BehaviorHost");
            var behaviors = global::Avalonia.Xaml.Interactivity.Interaction.GetBehaviors(host).OfType<TestCommandBehavior>().ToList();

            view.Show();
            try
            {
                Dispatcher.UIThread.RunJobs();

                Assert.AreEqual(2, behaviors.Count);
                foreach (var behavior in behaviors)
                {
                    Assert.IsInstanceOfType<IDelegateCommand>(behavior.Command);
                    behavior.Command.Execute(7);
                    Assert.AreEqual(7, viewModel.SavedValue);
                }
            }
            finally
            {
                view.Close();
            }
        }
    }
}
