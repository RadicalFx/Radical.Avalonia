using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Radical.Avalonia.Behaviors
{
    sealed class CueBannerAdorner : OverlayAdorner
    {
        public CueBannerAdorner(object content)
            : base(content, new TextBlock()
            {
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(6, 0, 0, 0),
                Opacity = 0.7
            })
        {
            IsHitTestVisible = false;
        }
    }
}
