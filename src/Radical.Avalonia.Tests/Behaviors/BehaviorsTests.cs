using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Avalonia.Behaviors;
using Radical.Avalonia.Input;
using System.Collections.Generic;
using System.Linq;

namespace Radical.Avalonia.Tests.Behaviors
{
    [TestClass]
    public class BehaviorsTests
    {
        static Window Show(Control content)
        {
            var window = new Window { Width = 300, Height = 300, Content = content };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            return window;
        }

        static (BehaviorsView, BehaviorsViewModel, Window) ShowBehaviorsView()
        {
            var viewModel = new BehaviorsViewModel();
            var view = new BehaviorsView { DataContext = viewModel };

            return (view, viewModel, Show(view));
        }

        static void PressKey(Window window, Key key, RawInputModifiers modifiers = RawInputModifiers.None)
        {
            window.KeyPress(key, modifiers, PhysicalKey.None, null);
            window.KeyRelease(key, modifiers, PhysicalKey.None, null);
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaTestMethod, TestCategory("Behaviors")]
        public void Focus_should_move_focus_to_the_element_using_the_key()
        {
            var (view, viewModel, _) = ShowBehaviorsView();

            viewModel.FocusedElement = "LastName";
            Dispatcher.UIThread.RunJobs();

            Assert.IsTrue(view.LastName.IsFocused);
        }

        [AvaloniaTestMethod, TestCategory("Behaviors")]
        public void Focus_should_report_the_focused_element_key()
        {
            var (view, viewModel, _) = ShowBehaviorsView();

            view.FirstName.Focus();
            Dispatcher.UIThread.RunJobs();
            Assert.AreEqual("FirstName", viewModel.FocusedElement);

            view.Search.Focus();
            Dispatcher.UIThread.RunJobs();
            Assert.IsNull(viewModel.FocusedElement);
        }

        [AvaloniaTestMethod, TestCategory("Behaviors")]
        public void TextBoxManager_should_execute_the_command_on_Enter()
        {
            var (view, viewModel, window) = ShowBehaviorsView();

            view.Search.Focus();
            PressKey(window, Key.Enter);

            var args = viewModel.Executions.Cast<TextBoxCommandArgs>().Single();
            Assert.AreEqual(Key.Enter, args.Key);
            Assert.AreEqual(KeyModifiers.None, args.Modifiers);
            Assert.AreEqual("search", args.CommandParameter);
        }

        [AvaloniaTestMethod, TestCategory("Behaviors")]
        public void TextBoxManager_should_execute_the_command_only_for_its_gestures()
        {
            var executions = new List<TextBoxCommandArgs>();
            var command = DelegateCommand.Create()
                .OnExecute(p => executions.Add((TextBoxCommandArgs)p))
                .AddKeyGesture(Key.F5, KeyModifiers.Control);
            var textBox = new TextBox();
            TextBoxManager.SetCommand(textBox, command);
            var window = Show(textBox);

            textBox.Focus();
            PressKey(window, Key.Enter);
            PressKey(window, Key.F5, RawInputModifiers.Control);

            var args = executions.Single();
            Assert.AreEqual(Key.F5, args.Key);
            Assert.AreEqual(KeyModifiers.Control, args.Modifiers);
        }

        [AvaloniaTestMethod, TestCategory("Behaviors")]
        public void TextBoxManager_should_stop_handling_keys_when_the_command_is_cleared()
        {
            var (view, viewModel, window) = ShowBehaviorsView();

            TextBoxManager.SetCommand(view.Search, null);
            view.Search.Focus();
            PressKey(window, Key.Enter);

            Assert.AreEqual(0, viewModel.Executions.Count);
        }

        [AvaloniaTestMethod, TestCategory("Behaviors")]
        public void TextBoxManager_should_select_the_text_on_focus()
        {
            var (view, _, _) = ShowBehaviorsView();
            view.Search.Text = "some text";

            view.Search.Focus();
            Dispatcher.UIThread.RunJobs();

            Assert.AreEqual("some text", view.Search.SelectedText);
        }

        [AvaloniaTestMethod, TestCategory("Behaviors")]
        public void TextBoxCommandBehavior_should_execute_the_command_on_Enter()
        {
            var executions = new List<TextBoxCommandArgs>();
            var textBox = new TextBox();
            Interaction.GetBehaviors(textBox).Add(new TextBoxCommandBehavior
            {
                Command = DelegateCommand.Create().OnExecute(p => executions.Add((TextBoxCommandArgs)p)),
                CommandParameter = 42
            });
            var window = Show(textBox);

            textBox.Focus();
            PressKey(window, Key.Enter);

            var args = executions.Single();
            Assert.AreEqual(Key.Enter, args.Key);
            Assert.AreEqual(42, args.CommandParameter);
        }

        [AvaloniaTestMethod, TestCategory("Behaviors")]
        public void Handle_should_execute_the_command_passing_in_the_requested_value()
        {
            var (view, viewModel, _) = ShowBehaviorsView();

            view.Save.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

            CollectionAssert.AreEqual(new object[] { "Save" }, viewModel.Executions);
        }

        [AvaloniaTestMethod, TestCategory("Behaviors")]
        public void MouseManager_should_report_pointer_positions()
        {
            var border = new Border { Background = global::Avalonia.Media.Brushes.White };
            MouseManager.SetOnMouseLeftButtonDown(border, new Point(-2, -2));
            var window = Show(border);

            window.MouseDown(new Point(10, 20), MouseButton.Left);
            window.MouseUp(new Point(10, 20), MouseButton.Left);
            window.MouseDown(new Point(30, 40), MouseButton.Right);
            window.MouseUp(new Point(30, 40), MouseButton.Right);
            Dispatcher.UIThread.RunJobs();

            Assert.AreEqual(new Point(10, 20), MouseManager.GetOnMouseLeftButtonDown(border));
            Assert.AreEqual(new Point(30, 40), MouseManager.GetOnMouseRightButtonDown(border));
        }

        [AvaloniaTestMethod, TestCategory("Behaviors")]
        public void ScrollViewerManager_should_sync_offsets_both_ways()
        {
            var viewer = new ScrollViewer
            {
                HorizontalScrollBarVisibility = global::Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
                Content = new Border { Width = 1000, Height = 1000 }
            };
            ScrollViewerManager.SetVerticalOffset(viewer, 50);
            ScrollViewerManager.SetHorizontalOffset(viewer, 30);
            Show(viewer);
            Assert.AreEqual(new Vector(30, 50), viewer.Offset);

            ScrollViewerManager.SetVerticalOffset(viewer, 70);
            Dispatcher.UIThread.RunJobs();
            Assert.AreEqual(new Vector(30, 70), viewer.Offset);

            viewer.Offset = new Vector(10, 100);
            Dispatcher.UIThread.RunJobs();
            Assert.AreEqual(10, ScrollViewerManager.GetHorizontalOffset(viewer));
            Assert.AreEqual(100, ScrollViewerManager.GetVerticalOffset(viewer));
        }

        [AvaloniaTestMethod, TestCategory("Behaviors")]
        public void ScrollIntoViewForListBox_should_bring_the_selected_item_into_view()
        {
            var listBox = new ListBox { ItemsSource = Enumerable.Range(0, 100).Select(i => $"Item {i}").ToList() };
            Interaction.GetBehaviors(listBox).Add(new ScrollIntoViewForListBox());
            Show(listBox);

            listBox.SelectedIndex = 90;
            Dispatcher.UIThread.RunJobs();

            var container = listBox.ContainerFromIndex(90);
            Assert.IsNotNull(container);
            var top = container.TranslatePoint(new Point(), listBox).Value.Y;
            Assert.IsTrue(top >= 0 && top < listBox.Bounds.Height, $"Item top {top} is outside the list box viewport.");
        }
    }
}
