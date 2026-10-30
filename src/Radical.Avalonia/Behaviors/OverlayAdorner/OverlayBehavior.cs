using Radical.Avalonia.Internals;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Radical.Avalonia.Behaviors
{
    /// <summary>
    /// A behavior that shows content in an adorner overlaying the associated control.
    /// </summary>
    public class OverlayBehavior : RadicalBehavior<Control>
    {
        static OverlayBehavior()
        {
            ContentProperty.Changed.AddClassHandler<OverlayBehavior>((s, e) => s.OnContentChanged());
            IsVisibleProperty.Changed.AddClassHandler<OverlayBehavior>((s, e) => s.OnIsVisibleChanged());
            IsHitTestVisibleProperty.Changed.AddClassHandler<OverlayBehavior>((s, e) => s.IsHitTestVisibleChanged());
            BackgroundProperty.Changed.AddClassHandler<OverlayBehavior>((s, e) => s.OnBackgroundChanged());
        }

        #region Styled Property: Content

        /// <summary>
        /// Content Styled property
        /// </summary>
        public static readonly StyledProperty<object> ContentProperty = AvaloniaProperty.Register<OverlayBehavior, object>(
            "Content");

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        #endregion

        #region Styled Property: IsVisible

        /// <summary>
        /// Identifies the <see cref="IsVisible"/> styled property.
        /// </summary>
        public static readonly StyledProperty<bool> IsVisibleProperty = AvaloniaProperty.Register<OverlayBehavior, bool>(
            "IsVisible",
            true);

        /// <summary>
        /// Gets or sets a value indicating whether the overlay is visible. Defaults to <c>true</c>.
        /// </summary>
        public bool IsVisible
        {
            get { return GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        #endregion

        #region Styled Property: DisableAdornedElement

        /// <summary>
        /// Identifies the <see cref="DisableAdornedElement"/> styled property.
        /// </summary>
        public static readonly StyledProperty<bool> DisableAdornedElementProperty = AvaloniaProperty.Register<OverlayBehavior, bool>(
            "DisableAdornedElement");

        /// <summary>
        /// Gets or sets a value indicating whether the associated control is disabled while the overlay is visible.
        /// </summary>
        public bool DisableAdornedElement
        {
            get { return GetValue(DisableAdornedElementProperty); }
            set { SetValue(DisableAdornedElementProperty, value); }
        }

        #endregion

        #region Styled Property: IsHitTestVisible

        /// <summary>
        /// Identifies the <see cref="IsHitTestVisible"/> styled property.
        /// </summary>
        public static readonly StyledProperty<bool> IsHitTestVisibleProperty = AvaloniaProperty.Register<OverlayBehavior, bool>(
            "IsHitTestVisible",
            true);

        /// <summary>
        /// Gets or sets a value indicating whether the overlay is hit test visible. Defaults to <c>true</c>.
        /// </summary>
        public bool IsHitTestVisible
        {
            get { return GetValue(IsHitTestVisibleProperty); }
            set { SetValue(IsHitTestVisibleProperty, value); }
        }

        #endregion

        #region Styled Property: Background

        /// <summary>
        /// Identifies the <see cref="Background"/> styled property.
        /// </summary>
        public static readonly StyledProperty<IBrush> BackgroundProperty = AvaloniaProperty.Register<OverlayBehavior, IBrush>(
            "Background");

        /// <summary>
        /// Gets or sets the brush used to paint the overlay background.
        /// </summary>
        public IBrush Background
        {
            get { return GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        #endregion

        private void OnBackgroundChanged()
        {
            if (adorner != null)
            {
                adorner.Background = Background;
            }
        }

        private void IsHitTestVisibleChanged()
        {
            if (adorner != null)
            {
                adorner.IsHitTestVisible = IsHitTestVisible;
            }
        }

        private void OnIsVisibleChanged()
        {
            Toggle();
        }

        private void OnContentChanged()
        {
            // the adorner content cannot be changed: recreate it
            HideAdorner();
            Toggle();
        }

        ContentOverlayAdorner adorner;
        private bool wasEnabled;

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();

            Toggle();
        }

        private void Toggle()
        {
            if (AssociatedObject == null)
            {
                return;
            }

            if (IsVisible && Content != null)
            {
                ShowAdorner();
            }
            else
            {
                HideAdorner();
            }
        }

        void ShowAdorner()
        {
            if (adorner == null)
            {
                adorner = new ContentOverlayAdorner(Content)
                {
                    IsHitTestVisible = IsHitTestVisible,
                    Background = Background
                };
                Adorners.Add(AssociatedObject, adorner);

                wasEnabled = AssociatedObject.IsEnabled;
                AssociatedObject.IsEnabled = !DisableAdornedElement;
            }
        }

        void HideAdorner()
        {
            if (adorner != null)
            {
                Adorners.Remove(AssociatedObject, adorner);
                adorner = null;

                AssociatedObject.IsEnabled = wasEnabled;
            }
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            HideAdorner();

            base.OnDetaching();
        }
    }
}
