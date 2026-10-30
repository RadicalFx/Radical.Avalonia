using Radical.Avalonia.Internals;
using Avalonia;
using Avalonia.Controls;

namespace Radical.Avalonia.Behaviors
{
    public static class Overlay
    {
        static Overlay()
        {
            ContentProperty.Changed.AddClassHandler<Control>(OnContentPropertyChanged);
        }

        public static readonly AttachedProperty<object> ContentProperty = AvaloniaProperty.RegisterAttached<Control, object>(
                                      "Content",
                                      typeof(Overlay));


        public static object GetContent(Control control)
        {
            return control.GetValue(ContentProperty);
        }

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
