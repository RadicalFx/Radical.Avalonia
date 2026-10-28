using Radical.Avalonia.ComponentModel;

namespace Radical.Avalonia.Bootstrap
{
    /// <summary>
    /// Allows a third party component to be notified when the
    /// application life-cycle state changes.
    /// </summary>
    [ToolkitComponentAttribute]
    public interface IExpectBootCallback
    {
        /// <summary>
        /// Called when application boot process is completed.
        /// </summary>
        void OnBootCompleted();
    }
}
