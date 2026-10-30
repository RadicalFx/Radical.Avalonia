using Avalonia.Input;

namespace Radical.Avalonia.Behaviors
{
    /// <summary>
    /// The DragLeave event arguments.
    /// </summary>
    public class DragLeaveArgs : DragDropOperationArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DragLeaveArgs" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="keyStates">The key states.</param>
        /// <param name="dropTarget">The drop target.</param>
        /// <param name="allowedEffects">The allowed effects.</param>
        public DragLeaveArgs(IDataTransfer data, KeyModifiers keyStates, object dropTarget, DragDropEffects allowedEffects)
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
