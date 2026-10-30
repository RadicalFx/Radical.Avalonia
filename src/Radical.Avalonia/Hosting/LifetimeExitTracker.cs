using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace Radical.Avalonia.Hosting
{
    /// <summary>
    /// Tracks whether the application lifetime is exiting. When it is, stopping the host
    /// must not shut down the lifetime again: the desktop lifetime would raise Exit again,
    /// and stopping the host from an Exit handler would recurse.
    /// </summary>
    class LifetimeExitTracker
    {
        /// <summary>
        /// Starts tracking the current application lifetime. Called when the host
        /// is built, to subscribe to Exit before the application Exit handlers.
        /// </summary>
        public LifetimeExitTracker()
        {
            if (Application.Current != null && RadicalApplication.LifetimeOf(Application.Current) is IControlledApplicationLifetime lifetime)
            {
                lifetime.Exit += (s, e) => IsExiting = true;
            }
        }

        public bool IsExiting { get; private set; }
    }
}
