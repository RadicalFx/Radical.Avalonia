using Avalonia.Xaml.Interactivity;
using System;
using Avalonia;
using Avalonia.Controls;

namespace Radical.Avalonia.Behaviors
{
    /// <summary>
    /// Notifies when the object a behavior is attached to is loaded.
    /// </summary>
    public interface INotifyAttachedOjectLoaded
    {
        /// <summary>
        /// Occurs when the attached object is loaded.
        /// </summary>
        event EventHandler AttachedObjectLoaded;
        /// <summary>
        /// Gets the attached object.
        /// </summary>
        /// <typeparam name="T">The expected type of the attached object.</typeparam>
        /// <returns>The attached object, or <c>null</c> if it is not of the requested type.</returns>
        T GetAttachedObject<T>() where T : Control;
    }

    /// <summary>
    /// Base class for Radical behaviors, notifies when the associated object is loaded.
    /// </summary>
    /// <typeparam name="T">The type of the control the behavior can be attached to.</typeparam>
    public abstract class RadicalBehavior<T> : Behavior<T>,
        INotifyAttachedOjectLoaded
        where T : Control
    {
        /// <summary>
        /// Called when the associated object is loaded.
        /// </summary>
        protected override void OnLoaded()
        {
            base.OnLoaded();
            OnAttachedObjectLoaded();
        }

        /// <inheritdoc/>
        public event EventHandler AttachedObjectLoaded;

        /// <summary>
        /// Raises the <see cref="AttachedObjectLoaded"/> event.
        /// </summary>
        protected virtual void OnAttachedObjectLoaded()
        {
            if (AttachedObjectLoaded != null)
            {
                AttachedObjectLoaded(this, EventArgs.Empty);
            }
        }

        /// <inheritdoc/>
        public T GetAttachedObject<T>() where T : Control
        {
            return AssociatedObject as T;
        }
    }
}