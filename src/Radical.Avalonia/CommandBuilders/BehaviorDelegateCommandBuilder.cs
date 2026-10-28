using Radical.Avalonia.Behaviors;
using Avalonia;
using Avalonia.Controls;

namespace Radical.Avalonia.CommandBuilders
{
    class BehaviorDelegateCommandBuilder : DelegateCommandBuilder
    {
        public override bool CanCreateCommand(string path, AvaloniaObject target)
        {
            return !string.IsNullOrEmpty(path) && target is INotifyAttachedOjectLoaded;
        }

        public override object GetDataContext(AvaloniaObject target)
        {
            return ((INotifyAttachedOjectLoaded)target)
                    .GetAttachedObject<Control>()
                    .DataContext;
        }
    }
}
