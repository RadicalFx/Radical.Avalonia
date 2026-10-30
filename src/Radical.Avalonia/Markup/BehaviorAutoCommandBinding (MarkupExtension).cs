using Radical.Avalonia.Behaviors;
using Radical.Avalonia.CommandBuilders;
using Radical.Avalonia.ComponentModel;
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace Radical.Avalonia.Markup
{
    /// <summary>
    /// An <see cref="AutoCommandBinding"/> to bind commands of behaviors: the command is generated
    /// once the object the behavior is attached to is loaded.
    /// </summary>
    public class BehaviorAutoCommandBinding : AutoCommandBinding
    {
        /// <inheritdoc/>
        protected override void OnProvideValue(IServiceProvider provider, object value)
        {

            if (TryGetTargetItems(provider, out AvaloniaObject fe, out AvaloniaProperty dp))
            {
                if (fe is INotifyAttachedOjectLoaded inab)
                {
                    void h(object s, EventArgs e)
                    {
                        inab.AttachedObjectLoaded -= h;
                        OnTargetLoaded(fe, dp);
                    }

                    inab.AttachedObjectLoaded += h;
                }
            }
        }

        /// <summary>
        /// Input bindings are not supported for behaviors: does nothing.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="source">The target as command source.</param>
        /// <param name="command">The command.</param>
        protected override void SetInputBindings(AvaloniaObject target, ICommandSource source, IDelegateCommand command)
        {
            //Not supported ?
        }

        /// <inheritdoc/>
        protected override DelegateCommandBuilder GetCommandBuilder()
        {
            return new BehaviorDelegateCommandBuilder();
        }
    }
}