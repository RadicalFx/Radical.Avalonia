using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace Radical.Avalonia.Behaviors
{
    sealed class BusyAdorner : OverlayAdorner
    {
        public BusyAdorner(object userContent)
            : base(userContent, new TextBlock()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            })
        {
            Background = new SolidColorBrush(Color.FromArgb(100, 220, 220, 220));
        }
    }
}
