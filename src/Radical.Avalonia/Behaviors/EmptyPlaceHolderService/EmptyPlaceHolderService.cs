using Radical.Avalonia.Internals;
using Radical.Linq;
using System.Linq;
using Avalonia;
using Avalonia.Controls;

namespace Radical.Avalonia.Behaviors
{
    /// <summary>
    /// Exposes an attached property to show placeholder content, as an adorner, on empty <see cref="ItemsControl"/> controls.
    /// </summary>
    public static class EmptyPlaceHolderService
    {
        #region Attached Property: Content

        /// <summary>
        /// Identifies the Content attached property: the content shown when the items control has no items.
        /// </summary>
        public static readonly AttachedProperty<object> ContentProperty = AvaloniaProperty.RegisterAttached<ItemsControl, object>(
                                      "Content",
                                      typeof(EmptyPlaceHolderService));


        /// <summary>
        /// Gets the placeholder content of the given items control.
        /// </summary>
        /// <param name="owner">The items control.</param>
        /// <returns>The placeholder content.</returns>
        public static object GetContent(ItemsControl owner)
        {
            return owner.GetValue(ContentProperty);
        }

        /// <summary>
        /// Sets the placeholder content of the given items control.
        /// </summary>
        /// <param name="owner">The items control.</param>
        /// <param name="value">The placeholder content.</param>
        public static void SetContent(ItemsControl owner, object value)
        {
            owner.SetValue(ContentProperty, value);
        }

        #endregion

        static EmptyPlaceHolderService()
        {
            ContentProperty.Changed.AddClassHandler<ItemsControl>((s, e) =>
            {
                // the content changed: replace the existing placeholder, if any
                RemoveEmptyContent(s);
                Update(s);
            });

            ItemsControl.ItemCountProperty.Changed.AddClassHandler<ItemsControl>((s, e) => Update(s));
        }

        static void Update(ItemsControl control)
        {
            var content = GetContent(control);
            if (content == null || DesignTimeHelper.GetIsInDesignMode())
            {
                // not managed by the empty placeholder service
                return;
            }

            if (control.ItemCount == 0)
            {
                ShowEmptyContent(control, content);
            }
            else
            {
                RemoveEmptyContent(control);
            }
        }

        static void RemoveEmptyContent(Control control)
        {
            Adorners.Get(control)
                .OfType<EmptyContentAdorner>()
                .ForEach(adorner => Adorners.Remove(control, adorner));
        }

        static void ShowEmptyContent(Control control, object content)
        {
            if (!Adorners.Get(control).OfType<EmptyContentAdorner>().Any())
            {
                Adorners.Add(control, new EmptyContentAdorner(content));
            }
        }
    }
}
