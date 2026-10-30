using System;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Radical.Avalonia.Behaviors
{
    /// <summary>
    /// A behavior that handles a routed event executing a command.
    /// </summary>
    public class Handle : RadicalBehavior<Control>
    {
        #region Styled Property: RoutedEvent

        /// <summary>
        /// Identifies the <see cref="RoutedEvent"/> styled property.
        /// </summary>
        public static readonly StyledProperty<RoutedEvent> RoutedEventProperty = AvaloniaProperty.Register<Handle, RoutedEvent>(
            "RoutedEvent");

        /// <summary>
        /// Gets or sets the routed event to handle.
        /// </summary>
        public RoutedEvent RoutedEvent
        {
            get { return GetValue(RoutedEventProperty); }
            set { SetValue(RoutedEventProperty, value); }
        }

        #endregion

        #region Styled Property: Command

        /// <summary>
        /// Identifies the <see cref="WithCommand"/> styled property.
        /// </summary>
        public static readonly StyledProperty<ICommand> WithCommandProperty = AvaloniaProperty.Register<Handle, ICommand>(
            "WithCommand");

        /// <summary>
        /// Gets or sets the command executed when the routed event is raised.
        /// </summary>
        public ICommand WithCommand
        {
            get { return GetValue(WithCommandProperty); }
            set { SetValue(WithCommandProperty, value); }
        }

        #endregion

        #region Styled Property: PassingIn

        /// <summary>
        /// Identifies the <see cref="PassingIn"/> styled property.
        /// </summary>
        public static readonly StyledProperty<string> PassingInProperty = AvaloniaProperty.Register<Handle, string>(
            "PassingIn");

        /// <summary>
        /// Gets or sets the command parameter: <c>$args</c>, <c>$this</c>, <c>$source</c>, or
        /// <c>$originalSource</c>, optionally followed by a property name, e.g. <c>$args.Key</c>.
        /// </summary>
        public string PassingIn
        {
            get { return GetValue(PassingInProperty); }
            set { SetValue(PassingInProperty, value); }
        }

        #endregion

        readonly EventHandler<RoutedEventArgs> handler = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Handle"/> class.
        /// </summary>
        public Handle()
        {
            handler = (s, e) =>
            {
                object args = null;
                if (!string.IsNullOrWhiteSpace(PassingIn))
                {
                    object referencedObject = null;

                    if (PassingIn.StartsWith("$args.", StringComparison.OrdinalIgnoreCase))
                    {
                        referencedObject = e;
                    }
                    else if (PassingIn.StartsWith("$this.", StringComparison.OrdinalIgnoreCase))
                    {
                        referencedObject = AssociatedObject;
                    }
                    else if (PassingIn.StartsWith("$source.", StringComparison.OrdinalIgnoreCase))
                    {
                        referencedObject = e.Source;
                    }
                    else if (PassingIn.StartsWith("$originalSource.", StringComparison.OrdinalIgnoreCase))
                    {
                        //Avalonia has no OriginalSource, Source already is the element that raised the event
                        referencedObject = e.Source;
                    }

                    if (referencedObject != null)
                    {
                        var indexOfFirstDot = PassingIn.IndexOf('.');

                        //TODO: add support for nested properties Foo.Bar.Property
                        var propertyPath = PassingIn.Substring(indexOfFirstDot + 1).Split('.');
                        var property = propertyPath.First();

                        args = referencedObject.GetType().GetProperty(property).GetValue(referencedObject, null);
                    }
                    else if (PassingIn.Equals("$args", StringComparison.OrdinalIgnoreCase))
                    {
                        args = e;
                    }
                    else if (PassingIn.Equals("$this", StringComparison.OrdinalIgnoreCase))
                    {
                        args = AssociatedObject;
                    }
                    else if (PassingIn.Equals("$source", StringComparison.OrdinalIgnoreCase))
                    {
                        args = e.Source;
                    }
                    else if (PassingIn.Equals("$originalSource", StringComparison.OrdinalIgnoreCase))
                    {
                        args = e.Source;
                    }
                }

                //to do add support for AutoCommandBinding with MethodFact?
                if (WithCommand.CanExecute(args))
                {
                    WithCommand.Execute(args);
                }
            };
        }

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.AddHandler(RoutedEvent, handler);
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.RemoveHandler(RoutedEvent, handler);
        }
    }
}
