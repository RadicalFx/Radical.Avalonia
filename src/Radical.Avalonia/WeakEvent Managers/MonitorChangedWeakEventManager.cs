using Radical.ComponentModel;
using System;
using Avalonia.Utilities;

namespace Radical.Avalonia
{
    /// <summary>
    /// A WeakEventManager for the MontiorChanged event.
    /// </summary>
    public static class MonitorChangedWeakEventManager
    {
        static readonly WeakEvent<IMonitor, EventArgs> changed = WeakEvent.Register<IMonitor>(
            (monitor, handler) => monitor.Changed += handler,
            (monitor, handler) => monitor.Changed -= handler);

        /// <summary>
        /// Adds the supplied listener to the Changed event of the given monitor.
        /// </summary>
        /// <param name="source">The monitor.</param>
        /// <param name="listener">The listener.</param>
        public static void AddListener(IMonitor source, IWeakEventSubscriber<EventArgs> listener)
        {
            changed.Subscribe(source, listener);
        }

        /// <summary>
        /// Removes the supplied listener from the Changed event of the given monitor.
        /// </summary>
        /// <param name="source">The monitor.</param>
        /// <param name="listener">The listener.</param>
        public static void RemoveListener(IMonitor source, IWeakEventSubscriber<EventArgs> listener)
        {
            changed.Unsubscribe(source, listener);
        }
    }
}
