using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Radical.Avalonia.ComponentModel;

namespace Radical.Avalonia.Regions
{
    /// <summary>
    /// Highlights a region, showing its type and name, when
    /// regions highlighting is turned on via the "hr" command line switch.
    /// </summary>
    sealed class RegionHilightAdorner : Border
    {
        public RegionHilightAdorner( IRegion region, IBrush brush )
        {
            IsHitTestVisible = false;
            BorderBrush = brush;
            BorderThickness = new Thickness( 2 );

            Child = new TextBlock()
            {
                FontStyle = FontStyle.Italic,
                Text = string.Format( "{0}, {1}", region.GetType().Name, region.Name ),
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness( 0, 0, 4, 4 ),
                TextTrimming = TextTrimming.CharacterEllipsis,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 10,
                Foreground = brush
            };
        }
    }
}
