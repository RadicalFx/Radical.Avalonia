using Avalonia;
using Avalonia.Controls;
using Radical.Avalonia.Behaviors;
using System.Windows.Input;

namespace Radical.Avalonia.Tests.Markup
{
    public class TestCommandBehavior : RadicalBehavior<Button>
    {
        public static readonly StyledProperty<ICommand> CommandProperty =
            AvaloniaProperty.Register<TestCommandBehavior, ICommand>(nameof(Command));

        public ICommand Command
        {
            get => GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
    }
}
