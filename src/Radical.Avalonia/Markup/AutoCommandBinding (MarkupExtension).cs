using Radical.Avalonia.Behaviors;
using Radical.Avalonia.CommandBuilders;
using Radical.Avalonia.ComponentModel;
using Radical.Avalonia.Input;
using System;
using System.Diagnostics;
using Avalonia;

namespace Radical.Avalonia.Markup
{
    public class AutoCommandBinding : CommandBinding
    {
        readonly static TraceSource logger = new TraceSource(typeof(AutoCommandBinding).FullName);

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCommandBinding" /> class.
        /// </summary>
        public AutoCommandBinding()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCommandBinding" /> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public AutoCommandBinding(string path)
            : base(path)
        {

        }

        /// <summary>
        /// Returns a placeholder command and hooks the target lifecycle: the actual
        /// command is generated from the data context method matching the path.
        /// </summary>
        /// <param name="provider">Object that can provide services for the markup
        /// extension.</param>
        /// <returns>
        /// A placeholder command, replaced once the data context is available.
        /// </returns>
        public override object ProvideValue(IServiceProvider provider)
        {
            /*
             * The runtime delegate command cannot be created until the
             * target data context is available: returning the decorated
             * binding would bind to a path that does not exist on the
             * data context. Hook the target lifecycle and return a fake
             * empty command, replaced once the data context is set or
             * the target is loaded.
             */
            base.ProvideValue(provider);

            var fakePlaceholder = DelegateCommand.Create();

            return fakePlaceholder;
        }

        protected override void OnDataContextChanged(AvaloniaObject obj, AvaloniaProperty targetProperty, object newValue, object oldValue)
        {
            base.OnDataContextChanged(obj, targetProperty, newValue, oldValue);

            if (obj.GetValue(targetProperty) is DelegateCommand actualCommand)
            {
                var actualCommandData = actualCommand.GetData<CommandData>();
                var actualMonitor = actualCommandData != null ? actualCommandData.Monitor : null;

                if (actualMonitor != null)
                {
                    actualCommand.RemoveMonitor(actualMonitor);
                    actualCommand.EvaluateCanExecute();
                }
            }

            if (!DesignTimeHelper.GetIsInDesignMode() && newValue != null)
            {
                var newCommand = GetCommand(obj, targetProperty);
            }
        }

        protected virtual DelegateCommandBuilder GetCommandBuilder()
        {
            return new DelegateCommandBuilder();
        }

        protected override IDelegateCommand GetCommand(AvaloniaObject target, AvaloniaProperty targetProperty)
        {
            var builder = GetCommandBuilder();

            // Avalonia bindings use UnsetValue, not null, when no source is set.
            var source = Source == AvaloniaProperty.UnsetValue ? null : Source;
            var dataContext = source ?? builder.GetDataContext(target);

            if (builder.CanCreateCommand(Path, target) && builder.TryGenerateCommandData(Path, dataContext, out CommandData commandData))
            {
                var command = builder.CreateCommand(commandData);
                target.SetValue(targetProperty, command);

                return command;
            }

            return null;
        }
    }
}