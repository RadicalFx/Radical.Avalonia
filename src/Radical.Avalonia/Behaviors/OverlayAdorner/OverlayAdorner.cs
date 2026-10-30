using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Media;

namespace Radical.Avalonia.Behaviors
{
    /// <summary>
    /// Base class for adorners displaying user content on top of the adorned
    /// element. The adorner layer sizes adorners to the adorned element bounds.
    /// </summary>
    abstract class OverlayAdorner : Border
    {
        protected OverlayAdorner(object content, TextBlock textTemplate)
        {
            Child = new ContentPresenter()
            {
                Content = content is string text ? Configure(textTemplate, text) : content
            };
        }

        static TextBlock Configure(TextBlock textBlock, string text)
        {
            textBlock.Text = text;
            textBlock.FontStyle = FontStyle.Italic;
            textBlock.TextTrimming = TextTrimming.CharacterEllipsis;

            return textBlock;
        }

        internal object UserContent => ((ContentPresenter)Child).Content;
    }
}
