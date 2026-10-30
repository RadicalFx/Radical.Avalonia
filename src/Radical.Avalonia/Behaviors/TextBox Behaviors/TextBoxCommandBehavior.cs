using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Radical.Avalonia.Behaviors
{
    /// <summary>
    /// A behavior that executes a command when keys are pressed in the associated <see cref="TextBox"/>.
    /// The command is executed if the pressed key matches one of the command gestures, or is Enter
    /// when the command has no gestures.
    /// </summary>
    public class TextBoxCommandBehavior : RadicalBehavior<TextBox>
    {
        #region Styled Property: Command

        /// <summary>
        /// Identifies the <see cref="Command"/> styled property.
        /// </summary>
        public static readonly StyledProperty<ICommand> CommandProperty = AvaloniaProperty.Register<TextBoxCommandBehavior, ICommand>(
            "Command");

        /// <summary>
        /// Gets or sets the command to execute.
        /// </summary>
        public ICommand Command
        {
            get { return GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        #endregion

        /// <summary>
        /// Gets or sets the parameter passed to the command, wrapped in a <see cref="TextBoxCommandArgs"/>.
        /// </summary>
        public object CommandParameter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the command target: the associated text box.
        /// </summary>
        public IInputElement CommandTarget
        {
            get { return AssociatedObject; }
        }

        readonly EventHandler<KeyEventArgs> onPreviewKeyDown;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBoxCommandBehavior"/> class.
        /// </summary>
        public TextBoxCommandBehavior()
        {
            onPreviewKeyDown = (s, e) => TextBoxCommandKeyHandler.Handle(Command, CommandParameter, e);
        }

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AddHandler(InputElement.KeyDownEvent, onPreviewKeyDown, RoutingStrategies.Tunnel);
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.RemoveHandler(InputElement.KeyDownEvent, onPreviewKeyDown);
        }
    }
}
