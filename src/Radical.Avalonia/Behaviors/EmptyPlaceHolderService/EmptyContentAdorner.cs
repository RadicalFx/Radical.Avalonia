using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Radical.Avalonia.Behaviors
{
    class EmptyContentAdorner : OverlayAdorner
    {
        public EmptyContentAdorner(object content)
            : base(content, new TextBlock()
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 25, 0, 0),
                Opacity = 0.7
            })
        {
            IsHitTestVisible = false;
        }
    }
}
