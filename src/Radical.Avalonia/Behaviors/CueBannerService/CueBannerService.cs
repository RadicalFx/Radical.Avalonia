using Radical.Avalonia.Internals;
using Radical.Linq;
using System.Linq;
using Avalonia;
using Avalonia.Controls;

namespace Radical.Avalonia.Behaviors
{
    /// <summary>
    /// Exposes an attached property to show a cue banner, as an adorner, on empty <see cref="TextBox"/> controls.
    /// </summary>
    public static class CueBannerService
    {
        #region CueBanner Attached Property

        /// <summary>
        /// Identifies the CueBanner attached property: the content shown when the text box is empty.
        /// </summary>
        public static readonly AttachedProperty<object> CueBannerProperty = AvaloniaProperty.RegisterAttached<TextBox, object>(
                                      "CueBanner",
                                      typeof(CueBannerService));

        /// <summary>
        /// Gets the cue banner content of the given text box.
        /// </summary>
        /// <param name="control">The text box.</param>
        /// <returns>The cue banner content.</returns>
        public static object GetCueBanner(TextBox control)
        {
            return control.GetValue(CueBannerProperty);
        }

        /// <summary>
        /// Sets the cue banner content of the given text box.
        /// </summary>
        /// <param name="control">The text box.</param>
        /// <param name="value">The cue banner content.</param>
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
