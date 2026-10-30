using Radical.Analytics;
using Radical.ComponentModel;
using Radical.Validation;
using Radical.Avalonia.Analytics;
using Radical.Avalonia.ComponentModel;
using System;
using System.Collections.Generic;
using Avalonia.Input;
using Avalonia.Utilities;
using System.Windows.Input;

namespace Radical.Avalonia.Input
{
    /// <summary>
    /// The default implementation of the IDelegateCommand interface.
    /// </summary>
    public class DelegateCommand : IDelegateCommand, IWeakEventSubscriber<EventArgs>
    {
        object _data;

        /// <summary>
        /// Used by the AutoCommandBinding...shuold be rewritten better :-)
        /// </summary>
        /// <param name="data">The data.</param>
        internal void SetData(object data)
        {
            _data = data;
        }

        /// <summary>
        /// Used by the AutoCommandBinding...shuold be rewritten better :-)
        /// </summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <returns>The data associated with this command.</returns>
        internal T GetData<T>()
        {
            return (T)_data;
        }

        /// <summary>
        /// An emtpy command, usefull as a placeholder.
        /// </summary>
        public static readonly IDelegateCommand Empty =
            Create()
            .OnCanExecute(o => false);

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <returns>The new command.</returns>
        public static IDelegateCommand Create()
        {
            return new DelegateCommand(null, null, string.Empty);
        }

        /// <summary>
        /// Creates a new command with the specified display text.
        /// </summary>
        /// <param name="displayText">The display text.</param>
        /// <returns>The new command.</returns>
        public static IDelegateCommand Create(string displayText)
        {
            return new DelegateCommand(null, null, displayText);
        }

        private Action<object> executeMethod = null;
        private Func<object, bool> canExecuteMethod = null;

        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raises <seealso cref="CanExecuteChanged"/> on the UI thread.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        private List<KeyBinding> inputBindings;

        /// <summary>
        /// Constructor. Initializes delegate command with Execute delegate and CanExecute delegate
        /// </summary>
        /// <param name="executeMethod">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
        /// <param name="canExecuteMethod">Delegate to execute when CanExecute is called on the command.  This can be null.</param>
        /// <param name="displayText">Text displayed by elements this command is bound to</param>
        public DelegateCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod, string displayText)
        {
            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
            DisplayText = displayText;
        }

        /// <summary>
        /// Sets the given Action as the delegate that must handle
        /// the commands execution logic.
        /// </summary>
        /// <param name="executeMethod">The delegate to execute at execution time.</param>
        /// <returns>
        /// An instance of the current command.
        /// </returns>
        public virtual IDelegateCommand OnExecute(Action<object> executeMethod)
        {
            this.executeMethod = executeMethod;
            return this;
        }

        /// <summary>
        /// Sets the given Action as the delegate that must handle
        /// the logic that determines whether the command can be executed or not.
        /// </summary>
        /// <param name="canExecuteMethod">The delegate to invoke.</param>
        /// <returns>
        /// An instance of the current command.
        /// </returns>
        public virtual IDelegateCommand OnCanExecute(Func<object, bool> canExecuteMethod)
        {
            this.canExecuteMethod = canExecuteMethod;
            return this;
        }

        /// <summary>
        /// Adds a new gesture to associate input bindings
        /// </summary>
        /// <param name="gesture">The key gesture.</param>
        /// <returns>This command instance, for chaining.</returns>
        public IDelegateCommand AddGesture(KeyGesture gesture)
        {
            if (inputBindings == null)
            {
                inputBindings = new List<KeyBinding>();
            }

            inputBindings.Add(new KeyBinding() { Command = this, Gesture = gesture });

            return this;
        }

        /// <summary>
        /// Adds a new key gesture to associate input bindings.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>This command instance, for chaining.</returns>
        public IDelegateCommand AddKeyGesture(Key key)
        {
            return AddKeyGesture(key, KeyModifiers.None);
        }

        /// <summary>
        /// Adds a new key gesture, with modifiers, to associate input bindings.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="mk">The key modifiers.</param>
        /// <returns>This command instance, for chaining.</returns>
        public IDelegateCommand AddKeyGesture(Key key, KeyModifiers mk)
        {
            if (inputBindings == null)
            {
                inputBindings = new List<KeyBinding>();
            }

            inputBindings.Add(new KeyBinding() { Command = this, Gesture = new KeyGesture(key, mk) });

            return this;
        }

        /// <summary>
        /// Gets command display text
        /// </summary>
        public string DisplayText
        {
            get;
            private set;
        }

        /// <summary>
        /// Command's associated input bindings
        /// </summary>
        public IList<KeyBinding> InputBindings
        {
            get { return inputBindings; }
        }

        ///<summary>
        ///Defines the method that determines whether the command can execute in its current state.
        ///</summary>
        ///<param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        ///<returns>
        ///true if this command can be executed; otherwise, false.
        ///</returns>
        public bool CanExecute(object parameter)
        {
            if (canExecuteMethod == null)
            {
                return true;
            }

            return canExecuteMethod(parameter);
        }

        ///<summary>
        ///Defines the method to be called when the command is invoked.
        ///</summary>
        ///<param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            if (executeMethod == null)
            {
                return;
            }

            executeMethod(parameter);

            if (onBeforeTracking != null)
            {
                var evt = new CommandEvent { Name = DisplayText };
                evt.Data.Add("parameter", parameter);
                onBeforeTracking(evt);
            }
        }

        Action<AnalyticsEvent> onBeforeTracking;

        /// <summary>
        /// Intercepts the analytics tracking information before sending to the analytics service.
        /// </summary>
        /// <param name="onBeforeTracking">The before tracking handler.</param>
        /// <returns>
        /// An instance of the current command.
        /// </returns>
        public IDelegateCommand WithTrackingPreview(Action<AnalyticsEvent> onBeforeTracking)
        {
            this.onBeforeTracking = onBeforeTracking;

            return this;
        }

        /// <summary>
        /// Raises <see cref="CanExecuteChanged"/> so every command invoker can re-query to check if the command can execute.
        /// <remarks>Note that this will trigger the execution of <see cref="CanExecute"/> once for each invoker.</remarks>
        /// </summary>
        public virtual void EvaluateCanExecute()
        {
            OnCanExecuteChanged();
        }

        /// <summary>
        /// Adds all the given triggers to the list of triggers.
        /// </summary>
        /// <param name="triggers">The triggers.</param>
        /// <returns>
        /// An instance of the current command.
        /// </returns>
        public IDelegateCommand AddMonitor(params IMonitor[] triggers)
        {
            if (triggers != null)
            {
                foreach (var monitor in triggers)
                {
                    AddMonitor(monitor);
                }
            }

            return this;
        }

        /// <summary>
        /// Adds a trigger monitor to the list of triggers.
        /// </summary>
        /// <param name="source">The source monitor.</param>
        /// <returns>
        /// An instance of the current command.
        /// </returns>
        public IDelegateCommand AddMonitor(IMonitor source)
        {
            Ensure.That(source).Named("source").IsNotNull();
            MonitorChangedWeakEventManager.AddListener(source, this);

            return this;
        }

        /// <summary>
        /// Removes the given monitor from the list of triggers.
        /// </summary>
        /// <param name="source">The monitor to remove.</param>
        /// <returns>
        /// An instance of the current command.
        /// </returns>
        public IDelegateCommand RemoveMonitor(IMonitor source)
        {
            Ensure.That(source).Named("source").IsNotNull();
            MonitorChangedWeakEventManager.RemoveListener(source, this);

            return this;
        }

        /// <summary>
        /// Called one of the trigger registered with this command
        /// raises the changed event.
        /// </summary>
        /// <param name="source">The source trigger.</param>
        protected virtual void OnTriggerChanged(IMonitor source)
        {
            EvaluateCanExecute();
        }

        void IWeakEventSubscriber<EventArgs>.OnEvent(object sender, WeakEvent ev, EventArgs e)
        {
            OnTriggerChanged((IMonitor)sender);
        }
    }
}
