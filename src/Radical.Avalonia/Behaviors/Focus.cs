using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Radical.Avalonia.Behaviors
{
    /// <summary>
    /// Moves the keyboard focus to the associated control when <see cref="ControlledBy"/> matches
    /// <see cref="UsingKey"/>, and sets <see cref="ControlledBy"/> when the control gets focus.
    /// </summary>
    public class Focus : RadicalBehavior<Control>
    {
        #region Styled Property: ControlledBy

        /// <summary>
        /// The controlled by property
        /// </summary>
        public static readonly StyledProperty<string> ControlledByProperty = AvaloniaProperty.Register<Focus, string>(
            "ControlledBy",
            defaultBindingMode: BindingMode.TwoWay );

        static Focus()
        {
            ControlledByProperty.Changed.AddClassHandler<Focus, string>( ( sender, e ) => sender.OnFocusChanged( e.NewValue.GetValueOrDefault() ) );
        }

        /// <summary>
        /// Gets or sets the controlled by.
        /// </summary>
        /// <value>
        /// The controlled by.
        /// </value>
        public string ControlledBy
        {
            get { return GetValue( ControlledByProperty ); }
            set { SetValue( ControlledByProperty, value ); }
        }

        private void OnFocusChanged( string focusKey )
        {
            if ( AssociatedObject != null && string.Equals( UsingKey, focusKey ) && AssociatedObject.Focusable && !AssociatedObject.IsFocused )
            {
                AssociatedObject.Focus();
            }
        }

        #endregion

        /// <summary>
        /// Called when [attached].
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.GotFocus += OnGotFocus;
            AssociatedObject.LostFocus += OnLostFocus;
        }

        /// <summary>
        /// Called when [detaching].
        /// </summary>
        protected override void OnDetaching()
        {
            AssociatedObject.GotFocus -= OnGotFocus;
            AssociatedObject.LostFocus -= OnLostFocus;

            base.OnDetaching();
        }

        void OnGotFocus( object sender, RoutedEventArgs e )
        {
            ControlledBy = UsingKey;
        }

        void OnLostFocus( object sender, RoutedEventArgs e )
        {
            if ( ControlledBy == UsingKey )
            {
                /*
                 * if, when we loose focus, the FocusedElementKey is still
                 * pointing to us means that the user has moved the focus
                 * to an element not managed by a behvior like this one, thus
                 * we set the FocusedElementKey to null so to detach and
                 * correctly react if the focus gets back.
                 */
                ControlledBy = null;
            }
        }

        /// <summary>
        /// Gets or sets the using key.
        /// </summary>
        /// <value>
        /// The using key.
        /// </value>
        public string UsingKey { get; set; }
    }
}
