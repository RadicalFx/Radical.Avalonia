using Avalonia.Xaml.Interactivity;
using System;
using Avalonia;
using Avalonia.Controls;

namespace Radical.Avalonia.Behaviors
{
    public interface INotifyAttachedOjectLoaded
    {
        event EventHandler AttachedObjectLoaded;
        T GetAttachedObject<T>() where T : Control;
    }

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

        public event EventHandler AttachedObjectLoaded;

        protected virtual void OnAttachedObjectLoaded()
        {
            if (AttachedObjectLoaded != null)
            {
                AttachedObjectLoaded(this, EventArgs.Empty);
            }
        }

        public T GetAttachedObject<T>() where T : Control
        {
            return AssociatedObject as T;
        }
    }
}