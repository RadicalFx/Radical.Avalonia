using Radical.Validation;
using System;
using Avalonia.Input;

namespace Radical.Avalonia.Behaviors
{
    /// <summary>
    /// The base abstract class used by Drag 'n' Drop event arguments.
    /// </summary>
    public abstract class DragDropOperationArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DragDropOperationArgs"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="keyStates">The key states.</param>
        /// <param name="dropTarget">The drop target.</param>
        protected DragDropOperationArgs(IDataTransfer data, KeyModifiers keyStates, object dropTarget)
        {
            Ensure.That(data).Named("data").IsNotNull();

            Data = data;
            KeyStates = keyStates;
            DropTarget = dropTarget;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data.</value>
        public IDataTransfer Data { get; private set; }

        /// <summary>
        /// Gets the key modifiers pressed during the operation.
        /// </summary>
        /// <value>The key states.</value>
        public KeyModifiers KeyStates { get; private set; }

        /// <summary>
        /// Gets or sets the drop target.
        /// </summary>
        /// <value>The drop target.</value>
        public object DropTarget { get; private set; }

        /// <summary>
        /// Gets the dragged object, set via the <c>DragDropManager.DataObject</c> attached property
        /// on a drag source whose <c>DragDropManager.DataObjectType</c> is the given format.
        /// </summary>
        /// <param name="format">The data object type.</param>
        /// <returns>The dragged object, or null if the data has no object of the given format.</returns>
        public object GetData(string format)
        {
            return Data.TryGetValue(DragDropManager.GetDataFormat(format));
        }

        /// <summary>
        /// Gets the dragged object, set via the <c>DragDropManager.DataObject</c> attached property
        /// on a drag source with no <c>DragDropManager.DataObjectType</c>, of the given type.
        /// </summary>
        /// <param name="format">The data object type.</param>
        /// <returns>The dragged object, or null if the data has no object of the given type.</returns>
        public object GetData(Type format)
        {
            return Data.TryGetValue(DragDropManager.GetDataFormat(format));
        }
    }
}
