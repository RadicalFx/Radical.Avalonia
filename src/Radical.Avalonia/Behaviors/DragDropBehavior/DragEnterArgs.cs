using Avalonia.Input;

namespace Radical.Avalonia.Behaviors
{
    /// <summary>
    /// The DragEnter event arguments.
    /// </summary>
    public class DragEnterArgs : DragDropOperationArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DragEnterArgs" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="keyStates">The key states.</param>
        /// <param name="dropTarget">The drop target.</param>
        /// <param name="allowedEffects">The allowed effects.</param>
        public DragEnterArgs(IDataTransfer data, KeyModifiers keyStates, object dropTarget, DragDropEffects allowedEffects)
            : base(data, keyStates, dropTarget)
        {
            AllowedEffects = allowedEffects;
        }

        /// <summary>
        /// Gets the allowed effects.
        /// </summary>
        /// <value>The allowed effects.</value>
        public DragDropEffects AllowedEffects { get; private set; }
    }
}
