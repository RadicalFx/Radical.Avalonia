using Avalonia.Controls;
using Avalonia.Threading;

namespace Radical.Avalonia.Behaviors
{
    public class ScrollIntoViewForListBox : RadicalBehavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem != null)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    listBox.UpdateLayout();
                    if (listBox.SelectedItem != null)
                        listBox.ScrollIntoView(listBox.SelectedItem);
                });
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
        }
    }
}
