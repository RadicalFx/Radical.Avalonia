using Avalonia;
using Avalonia.Controls;

namespace Radical.Avalonia.Messaging
{
    /// <summary>
    /// Domain event that identifies that a view has been loaded.
    /// </summary>
    public class ViewLoaded
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewLoaded" /> class.
        /// </summary>
        /// <param name="view">The view.</param>
        public ViewLoaded(AvaloniaObject view)
        {
            View = view;
        }

        /// <summary>
        /// Gets the view.
        /// </summary>
        public AvaloniaObject View { get; private set; }
    }
}
