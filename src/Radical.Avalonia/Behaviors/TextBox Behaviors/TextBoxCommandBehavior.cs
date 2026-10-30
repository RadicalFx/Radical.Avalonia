using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Radical.Avalonia.Behaviors
{
    public class TextBoxCommandBehavior : RadicalBehavior<TextBox>
    {
        #region Styled Property: Command

        public static readonly StyledProperty<ICommand> CommandProperty = AvaloniaProperty.Register<TextBoxCommandBehavior, ICommand>(
            "Command");

        public ICommand Command
        {
            get { return GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        #endregion

        public object CommandParameter
        {
            get;
            set;
        }

        public IInputElement CommandTarget
        {
            get { return AssociatedObject; }
        }

        readonly EventHandler<KeyEventArgs> onPreviewKeyDown;

        public TextBoxCommandBehavior()
        {
            onPreviewKeyDown = (s, e) => TextBoxCommandKeyHandler.Handle(Command, CommandParameter, e);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AddHandler(InputElement.KeyDownEvent, onPreviewKeyDown, RoutingStrategies.Tunnel);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.RemoveHandler(InputElement.KeyDownEvent, onPreviewKeyDown);
        }
    }
}
