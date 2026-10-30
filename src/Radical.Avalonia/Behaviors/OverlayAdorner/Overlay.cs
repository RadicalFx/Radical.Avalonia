using Radical.Avalonia.Internals;
using Avalonia;
using Avalonia.Controls;

namespace Radical.Avalonia.Behaviors
{
    /// <summary>
    /// Exposes an attached property to show content in an adorner overlaying the control.
    /// </summary>
    public static class Overlay
    {
        static Overlay()
        {
            ContentProperty.Changed.AddClassHandler<Control>(OnContentPropertyChanged);
        }

        /// <summary>
        /// Identifies the Content attached property: the content displayed in the overlay.
        /// </summary>
        public static readonly AttachedProperty<object> ContentProperty = AvaloniaProperty.RegisterAttached<Control, object>(
                                      "Content",
                                      typeof(Overlay));


        /// <summary>
        /// Gets the overlay content of the given control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns>The overlay content.</returns>
        public static object GetContent(Control control)
        {
            return control.GetValue(ContentProperty);
        }

        /// <summary>
        /// Sets the overlay content of the given control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="value">The overlay content.</param>
        public static void SetContent(Control control, object value)
        {
            control.SetValue(ContentProperty, value);
        }

        static readonly AttachedProperty<ContentOverlayAdorner> OverlayAdornerProperty = AvaloniaProperty.RegisterAttached<Control, ContentOverlayAdorner>(
                                      "OverlayAdorner",
                                      typeof(Overlay));

        static void OnContentPropertyChanged(Control control, AvaloniaPropertyChangedEventArgs args)
        {
            if (!DesignTimeHelper.GetIsInDesignMode())
            {
                var a = control.GetValue(OverlayAdornerProperty);
                if (a != null)
                {
                    Adorners.Remove(control, a);
                    control.SetValue(OverlayAdornerProperty, null);
                }

                var content = GetContent(control);
                if (content != null)
                {
                    a = new ContentOverlayAdorner(content);
                    Adorners.Add(control, a);
                    control.SetValue(OverlayAdornerProperty, a);
                }
            }
        }
    }
}
