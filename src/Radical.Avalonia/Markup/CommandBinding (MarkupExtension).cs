using Radical.Avalonia.Behaviors;
using Radical.Avalonia.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;

namespace Radical.Avalonia.Markup
{
    /// <summary>
    /// A binding to a command that, once the target is loaded, registers the command key
    /// bindings on the root element.
    /// </summary>
    public class CommandBinding : BindingDecoratorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBinding" /> class.
        /// </summary>
        public CommandBinding()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBinding" /> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public CommandBinding(string path)
            : base(path)
        {

        }

        /// <summary>
        /// Returns the decorated binding and, at runtime, hooks the target
        /// lifecycle to register the command input bindings once the target is loaded.
        /// </summary>
        /// <param name="provider">Object that can provide services for the markup
        /// extension.</param>
        /// <returns>
        /// The object value to set on the property where the extension is applied.
        /// </returns>
        public override object ProvideValue(IServiceProvider provider)
        {
            var b = base.ProvideValue(provider);

            OnProvideValue(provider, b);

            return b;
        }

        /// <summary>
        /// Called when the value is provided, hooks the target lifecycle.
        /// </summary>
        /// <param name="provider">The service provider.</param>
        /// <param name="value">The provided value.</param>
        protected virtual void OnProvideValue(IServiceProvider provider, object value)
        {
            if (!DesignTimeHelper.GetIsInDesignMode())
            {
                if (TryGetTargetItems(provider, out Control fe, out AvaloniaProperty dp))
                {
                    void reh(object s, RoutedEventArgs e)
                    {
                        fe.Loaded -= reh;
                        OnTargetLoaded(fe, dp);
                    }

                    fe.Loaded += reh;

                    var oldDataContext = fe.DataContext;
                    fe.DataContextChanged += (s, e) =>
                    {
                        var newDataContext = fe.DataContext;
                        OnDataContextChanged(fe, dp, newDataContext, oldDataContext);
                        oldDataContext = newDataContext;
                    };
                }
            }
        }

        /// <summary>
        /// Called when the data context of the target changes.
        /// </summary>
        /// <param name="obj">The target.</param>
        /// <param name="targetProperty">The target property.</param>
        /// <param name="newValue">The new data context.</param>
        /// <param name="oldValue">The old data context.</param>
        protected virtual void OnDataContextChanged(AvaloniaObject obj, AvaloniaProperty targetProperty, object newValue, object oldValue)
        {

        }

        /// <summary>
        /// Called when the target is loaded.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="targetProperty">The target property.</param>
        protected virtual void OnTargetLoaded(AvaloniaObject target, AvaloniaProperty targetProperty)
        {
            var source = target as ICommandSource;
            var command = GetCommand(target, targetProperty);

            SetInputBindings(target, source, command);
        }

        /// <summary>
        /// Adds the command key bindings to the root element of the given target, so that
        /// they are available regardless of the focused element.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="source">The target as command source.</param>
        /// <param name="command">The command.</param>
        protected virtual void SetInputBindings(AvaloniaObject target, ICommandSource source, IDelegateCommand command)
        {
            if (source != null && command != null && command.InputBindings != null)
            {
                var rootElement = GetRootElement(target as Control);
                foreach (var ib in command.InputBindings)
                {
                    if (ib.CommandParameter != source.CommandParameter)
                    {
                        ib.CommandParameter = source.CommandParameter;
                    }

                    if (!rootElement.KeyBindings.Contains(ib))
                    {
                        rootElement.KeyBindings.Add(ib);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the command currently set on the target property.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="targetProperty">The target property.</param>
        /// <returns>The command, if any.</returns>
        protected virtual IDelegateCommand GetCommand(AvaloniaObject target, AvaloniaProperty targetProperty)
        {
            return target.GetValue(targetProperty) as IDelegateCommand;
        }

        /// <summary>
        /// Gets the root element of the given element.
        /// </summary>
        /// <param name="fe">The element.</param>
        /// <returns>The root element.</returns>
        protected virtual Control GetRootElement(Control fe)
        {
            if (fe.Parent == null)
            {
                return fe;
            }
            else
            {
                return GetRootElement(fe.Parent as Control);
            }
        }
    }
}
