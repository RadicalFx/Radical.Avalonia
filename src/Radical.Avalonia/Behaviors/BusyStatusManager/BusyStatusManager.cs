using Radical.Avalonia.Internals;
using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;

namespace Radical.Avalonia.Behaviors
{
    /// <summary>
    /// Exposes attached properties to show busy content as an adorner, and disable the
    /// control, while the control status is <see cref="BusyStatus.Busy"/>.
    /// </summary>
    public static class BusyStatusManager
    {
        static BusyStatusManager()
        {
            ContentProperty.Changed.AddClassHandler<Control>(OnPropertyChanged);
            StatusProperty.Changed.AddClassHandler<Control>(OnPropertyChanged);
        }

        /// <summary>
        /// Identifies the Content attached property: the content displayed by the busy adorner.
        /// </summary>
        /// <remarks>The content cannot be changed once the control is loaded.</remarks>
        public static readonly AttachedProperty<object> ContentProperty = AvaloniaProperty.RegisterAttached<Control, object>(
                                      "Content",
                                      typeof(BusyStatusManager));


        /// <summary>
        /// Gets the content displayed while the given control is busy.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns>The busy content.</returns>
        public static object GetContent(Control control)
        {
            return control.GetValue(ContentProperty);
        }

        /// <summary>
        /// Sets the content displayed while the given control is busy.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="value">The busy content.</param>
        public static void SetContent(Control control, object value)
        {
            control.SetValue(ContentProperty, value);
        }

        /// <summary>
        /// Identifies the Status attached property: the busy status of the control. Defaults to <see cref="BusyStatus.Idle"/>.
        /// </summary>
        public static readonly AttachedProperty<BusyStatus> StatusProperty = AvaloniaProperty.RegisterAttached<Control, BusyStatus>(
                              "Status",
                              typeof(BusyStatusManager),
                              BusyStatus.Idle);

        /// <summary>
        /// Gets the busy status of the given control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns>The busy status.</returns>
        public static BusyStatus GetStatus(Control control)
        {
            return control.GetValue(StatusProperty);
        }

        /// <summary>
        /// Sets the busy status of the given control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="value">The busy status.</param>
        public static void SetStatus(Control control, BusyStatus value)
        {
            control.SetValue(StatusProperty, value);
        }

        static void HandleStatusChanged(Control element)
        {
            var content = GetContent(element);
            var status = GetStatus(element);
            var busyAdorner = Adorners.Get(element).OfType<BusyAdorner>().FirstOrDefault();

            switch (status)
            {
                case BusyStatus.Idle:

                    element.IsEnabled = true;

                    if (busyAdorner != null)
                    {
                        Adorners.Remove(element, busyAdorner);
                    }

                    break;

                case BusyStatus.Busy:

                    element.IsEnabled = false;
                    if (content != null && busyAdorner == null)
                    {
                        Adorners.Add(element, new BusyAdorner(content));
                    }

                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        static void OnPropertyChanged(Control control, AvaloniaPropertyChangedEventArgs e)
        {
            if (!DesignTimeHelper.GetIsInDesignMode())
            {
                if (e.Property == StatusProperty)
                {
                    HandleStatusChanged(control);
                }
                else if (control.IsLoaded && e.Property == ContentProperty)
                {
                    throw new NotSupportedException("BusyStatusManager: Content property cannot be changed at runtime.");
                }
                else if (e.Property == ContentProperty)
                {
                    // Content can be set after Status, e.g. depending on the XAML attributes order
                    HandleStatusChanged(control);
                }
            }
        }
    }
}
