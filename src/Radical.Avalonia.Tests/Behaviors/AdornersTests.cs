using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Avalonia.Behaviors;
using Radical.Avalonia.Converters;
using System;
using System.Globalization;
using System.Linq;

namespace Radical.Avalonia.Tests.Behaviors
{
    [TestClass]
    public class AdornersTests
    {
        static Window Show(Control content)
        {
            var window = new Window { Width = 300, Height = 400, Content = content };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            return window;
        }

        static (AdornersView, AdornersViewModel) ShowAdornersView()
        {
            var viewModel = new AdornersViewModel();
            var view = new AdornersView { DataContext = viewModel };
            Show(view);

            return (view, viewModel);
        }

        /// <summary>
        /// The texts displayed by the adorners of the given element.
        /// </summary>
        static string[] AdornerTexts(Control element)
        {
            Dispatcher.UIThread.RunJobs();
            TopLevel.GetTopLevel(element)?.UpdateLayout();

            var layer = AdornerLayer.GetAdornerLayer(element);
            Assert.IsNotNull(layer);

            return layer.Children
                .Where(c => AdornerLayer.GetAdornedElement(c) == element)
                .SelectMany(c => c.GetSelfAndVisualDescendants())
                .OfType<TextBlock>()
                .Select(t => t.Text)
                .ToArray();
        }

        [AvaloniaTestMethod, TestCategory("Behaviors")]
        public void BusyStatusManager_should_show_the_busy_content_and_disable_the_element()
        {
            var (view, viewModel) = ShowAdornersView();
            CollectionAssert.AreEqual(new string[0], AdornerTexts(view.Busy));

            viewModel.IsBusy = true;
            CollectionAssert.AreEqual(new[] { "Loading..." }, AdornerTexts(view.Busy));
            Assert.IsFalse(view.Busy.IsEnabled);

            viewModel.IsBusy = false;
            CollectionAssert.AreEqual(new string[0], AdornerTexts(view.Busy));
            Assert.IsTrue(view.Busy.IsEnabled);
        }

        [AvaloniaTestMethod, TestCategory("Behaviors")]
        public void BusyStatusManager_should_support_content_set_after_status()
        {
            var border = new Border();
            BusyStatusManager.SetStatus(border, BusyStatus.Busy);
            BusyStatusManager.SetContent(border, "Loading...");
            Show(border);

            CollectionAssert.AreEqual(new[] { "Loading..." }, AdornerTexts(border));
        }

        [AvaloniaTestMethod, TestCategory("Behaviors")]
        public void BusyStatusManager_should_not_allow_changing_content_at_runtime()
        {
            var (view, _) = ShowAdornersView();

            Assert.ThrowsExactly<NotSupportedException>(() => BusyStatusManager.SetContent(view.Busy, "Other"));
        }

        [AvaloniaTestMethod, TestCategory("Behaviors")]
        public void OverlayBehavior_should_toggle_the_overlay_and_restore_the_element_state()
        {
            var (view, viewModel) = ShowAdornersView();
            CollectionAssert.AreEqual(new string[0], AdornerTexts(view.Overlaid));

            viewModel.IsOverlayVisible = true;
            CollectionAssert.AreEqual(new[] { "Overlay" }, AdornerTexts(view.Overlaid));
            Assert.IsFalse(view.Overlaid.IsEnabled);

            viewModel.IsOverlayVisible = false;
            CollectionAssert.AreEqual(new string[0], AdornerTexts(view.Overlaid));
            Assert.IsTrue(view.Overlaid.IsEnabled);
        }

        [AvaloniaTestMethod, TestCategory("Behaviors")]
        public void OverlayBehavior_should_remove_the_overlay_when_detached()
        {
            var border = new Border();
            var behavior = new OverlayBehavior { Content = "Overlay" };
            Interaction.GetBehaviors(border).Add(behavior);
            Show(border);
            CollectionAssert.AreEqual(new[] { "Overlay" }, AdornerTexts(border));

            Interaction.GetBehaviors(border).Remove(behavior);

            CollectionAssert.AreEqual(new string[0], AdornerTexts(border));
        }

        [AvaloniaTestMethod, TestCategory("Behaviors")]
        public void Overlay_should_replace_the_overlay_when_the_content_changes()
        {
            var border = new Border();
            Overlay.SetContent(border, "First");
            Show(border);
            CollectionAssert.AreEqual(new[] { "First" }, AdornerTexts(border));

            Overlay.SetContent(border, "Second");
            CollectionAssert.AreEqual(new[] { "Second" }, AdornerTexts(border));

            Overlay.SetContent(border, null);
            CollectionAssert.AreEqual(new string[0], AdornerTexts(border));
        }

        [AvaloniaTestMethod, TestCategory("Behaviors")]
        public void Adorners_on_the_same_element_should_coexist()
        {
            var border = new Border();
            Overlay.SetContent(border, "Overlay");
            BusyStatusManager.SetContent(border, "Loading...");
            BusyStatusManager.SetStatus(border, BusyStatus.Busy);
            Show(border);

            CollectionAssert.AreEquivalent(new[] { "Overlay", "Loading..." }, AdornerTexts(border));

            BusyStatusManager.SetStatus(border, BusyStatus.Idle);
            CollectionAssert.AreEqual(new[] { "Overlay" }, AdornerTexts(border));
        }

        [AvaloniaTestMethod, TestCategory("Behaviors")]
        public void Adorners_should_follow_the_element_in_and_out_of_the_visual_tree()
        {
            var border = new Border();
            Overlay.SetContent(border, "Overlay");
            var panel = new StackPanel();
            Show(panel);
            var layer = AdornerLayer.GetAdornerLayer(panel);

            panel.Children.Add(border);
            CollectionAssert.AreEqual(new[] { "Overlay" }, AdornerTexts(border));

            panel.Children.Remove(border);
            Dispatcher.UIThread.RunJobs();
            Assert.IsFalse(layer.Children.Any(c => AdornerLayer.GetAdornedElement(c) == border));
        }

        [AvaloniaTestMethod, TestCategory("Behaviors")]
        public void CueBannerService_should_show_the_cue_banner_only_when_the_text_is_empty()
        {
            var (view, _) = ShowAdornersView();
            CollectionAssert.AreEqual(new[] { "Search..." }, AdornerTexts(view.Search));

            view.Search.Text = "radical";
            CollectionAssert.AreEqual(new string[0], AdornerTexts(view.Search));

            view.Search.Text = string.Empty;
            CollectionAssert.AreEqual(new[] { "Search..." }, AdornerTexts(view.Search));

            view.Search.IsVisible = false;
            CollectionAssert.AreEqual(new string[0], AdornerTexts(view.Search));
        }

        [AvaloniaTestMethod, TestCategory("Behaviors")]
        public void EmptyPlaceHolderService_should_show_the_placeholder_only_when_there_are_no_items()
        {
            var (view, viewModel) = ShowAdornersView();
            CollectionAssert.AreEqual(new[] { "No items" }, AdornerTexts(view.Items));

            viewModel.Items.Add("Item");
            CollectionAssert.AreEqual(new string[0], AdornerTexts(view.Items));

            viewModel.Items.Clear();
            CollectionAssert.AreEqual(new[] { "No items" }, AdornerTexts(view.Items));
        }

        [TestMethod, TestCategory("Converters")]
        public void BooleanBusyStatusConverter_should_convert_both_ways()
        {
            var sut = new BooleanBusyStatusConverter();

            Assert.AreEqual(BusyStatus.Busy, sut.Convert(true, typeof(BusyStatus), null, CultureInfo.InvariantCulture));
            Assert.AreEqual(BusyStatus.Idle, sut.Convert(false, typeof(BusyStatus), null, CultureInfo.InvariantCulture));
            Assert.AreEqual(true, sut.ConvertBack(BusyStatus.Busy, typeof(bool), null, CultureInfo.InvariantCulture));
            Assert.AreEqual(false, sut.ConvertBack(BusyStatus.Idle, typeof(bool), null, CultureInfo.InvariantCulture));
        }
    }
}
