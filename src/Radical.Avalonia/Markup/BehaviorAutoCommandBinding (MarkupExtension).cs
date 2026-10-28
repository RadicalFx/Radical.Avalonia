using Radical.Avalonia.Behaviors;
using Radical.Avalonia.CommandBuilders;
using Radical.Avalonia.ComponentModel;
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace Radical.Avalonia.Markup
{
    public class BehaviorAutoCommandBinding : AutoCommandBinding
    {
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

        protected override void SetInputBindings(AvaloniaObject target, ICommandSource source, IDelegateCommand command)
        {
            //Not supported ?
        }

        protected override DelegateCommandBuilder GetCommandBuilder()
        {
            return new BehaviorDelegateCommandBuilder();
        }
    }
}