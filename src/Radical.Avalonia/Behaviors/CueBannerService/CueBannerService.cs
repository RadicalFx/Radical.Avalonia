using Radical.Avalonia.Internals;
using Radical.Linq;
using System.Linq;
using Avalonia;
using Avalonia.Controls;

namespace Radical.Avalonia.Behaviors
{
    public static class CueBannerService
    {
        #region CueBanner Attached Property

        public static readonly AttachedProperty<object> CueBannerProperty = AvaloniaProperty.RegisterAttached<TextBox, object>(
                                      "CueBanner",
                                      typeof(CueBannerService));

        public static object GetCueBanner(TextBox control)
        {
            return control.GetValue(CueBannerProperty);
        }

        public static void SetCueBanner(TextBox control, object value)
        {
            control.SetValue(CueBannerProperty, value);
        }

        #endregion

        static CueBannerService()
        {
            CueBannerProperty.Changed.AddClassHandler<TextBox>((s, e) =>
            {
                if (!DesignTimeHelper.GetIsInDesignMode())
                {
                    // the content changed: replace the existing cue banner, if any
                    RemoveCueBanner(s);
                    HandleShowRequest(s);
                }
            });

            TextBox.TextProperty.Changed.AddClassHandler<TextBox>((s, e) => HandleShowRequest(s));
            Visual.IsVisibleProperty.Changed.AddClassHandler<TextBox>((s, e) => HandleShowRequest(s));
        }

        static void HandleShowRequest(TextBox control)
        {
            var cueBanner = GetCueBanner(control);
            if (cueBanner == null || DesignTimeHelper.GetIsInDesignMode())
            {
                // not managed by the cue banner service
                return;
            }

            if (ShouldShowCueBanner(control))
            {
                ShowCueBanner(control, cueBanner);
            }
            else
            {
                RemoveCueBanner(control);
            }
        }

        static void RemoveCueBanner(Control control)
        {
            Adorners.Get(control)
                .OfType<CueBannerAdorner>()
                .ForEach(adorner => Adorners.Remove(control, adorner));
        }

        static void ShowCueBanner(Control control, object content)
        {
            if (!Adorners.Get(control).OfType<CueBannerAdorner>().Any())
            {
                Adorners.Add(control, new CueBannerAdorner(content));
            }
        }

        static bool ShouldShowCueBanner(TextBox c)
        {
            return c.IsVisible && string.IsNullOrEmpty(c.Text);
        }
    }
}
