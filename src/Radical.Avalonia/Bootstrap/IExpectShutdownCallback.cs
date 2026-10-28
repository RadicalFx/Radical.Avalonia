using Radical.Avalonia.ComponentModel;

namespace Radical.Avalonia.Bootstrap
{
    /// <summary>
    /// Allows a third party component to be notified when the
    /// application life-cycle state changes.
    /// </summary>
    [ToolkitComponentAttribute]
    public interface IExpectShutdownCallback
    {
        /// <summary>
        /// Called when the application shuts down.
        /// </summary>
        /// <param name="reason">The reason.</param>
        void OnShutdown( ApplicationShutdownReason reason );
    }
}
