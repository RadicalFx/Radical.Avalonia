using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Radical.Avalonia.Behaviors
{
    /// <summary>
    /// Exposes attached properties that capture, as two-way bindable points, the position
    /// of the mouse when buttons are pressed on a control.
    /// </summary>
    public class MouseManager
    {

        static readonly EventHandler<PointerPressedEventArgs> onPointerPressed;
        static readonly EventHandler<TappedEventArgs> onMouseDoubleClick;
        static readonly EventHandler<ContextRequestedEventArgs> onContextMenuOpening;

        static MouseManager()
        {
            onPointerPressed = (s, e) =>
            {
                var ctrl = (Control)s;
                var point = e.GetCurrentPoint(ctrl);

                if (point.Properties.IsLeftButtonPressed)
                {
                    SetOnMouseLeftButtonDown(ctrl, point.Position);
                }
                else if (point.Properties.IsRightButtonPressed)
                {
                    SetOnMouseRightButtonDown(ctrl, point.Position);
                }
            };

            onMouseDoubleClick = (s, e) =>
            {
                var ctrl = (Control)s;
                SetOnMouseDoubleClick(ctrl, e.GetPosition(ctrl));
            };

            onContextMenuOpening = (s, e) =>
            {
                var ctrl = (Control)s;
                if (e.TryGetPosition(ctrl, out var p))
                {
                    SetOnContextMenuOpening(ctrl, p);
                }
            };

            OnMouseLeftButtonDownProperty.Changed.AddClassHandler<Control>(OnChanged);
            OnMouseRightButtonDownProperty.Changed.AddClassHandler<Control>(OnChanged);
            OnMouseDoubleClickProperty.Changed.AddClassHandler<Control>(OnChanged);
            OnContextMenuOpeningProperty.Changed.AddClassHandler<Control>(OnChanged);
        }

        #region Attached Property: IsSetupProperty

        static readonly AttachedProperty<bool> IsSetupProperty = AvaloniaProperty.RegisterAttached<Control, bool>(
                             "IsSetup",
                             typeof(MouseManager));

        private static void OnChanged(Control ctrl, AvaloniaPropertyChangedEventArgs e)
        {
            var isSetup = ctrl.GetValue(IsSetupProperty);

            if (!isSetup && !DesignTimeHelper.GetIsInDesignMode())
            {
                ctrl.AddHandler(InputElement.PointerPressedEvent, onPointerPressed);
                ctrl.AddHandler(InputElement.DoubleTappedEvent, onMouseDoubleClick);
                ctrl.AddHandler(Control.ContextRequestedEvent, onContextMenuOpening);
                ctrl.SetValue(IsSetupProperty, true);
            }

        }

        #endregion

        #region Attached Property: OnMouseLeftButtonDownProperty


        /// <summary>
        /// Identifies the OnMouseLeftButtonDown attached property: the position, relative to the control,
        /// where the left mouse button was last pressed. Defaults to (-1, -1).
        /// </summary>
        public static readonly AttachedProperty<Point> OnMouseLeftButtonDownProperty = AvaloniaProperty.RegisterAttached<Control, Point>(
                             "OnMouseLeftButtonDown",
                             typeof(MouseManager),
                             new Point(-1, -1),
                             defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// Gets the position where the left mouse button was last pressed on the given control.
        /// </summary>
        /// <param name="owner">The control.</param>
        /// <returns>The position, relative to the control.</returns>
        public static Point GetOnMouseLeftButtonDown(Control owner)
        {
            return owner.GetValue(OnMouseLeftButtonDownProperty);
        }

        /// <summary>
        /// Sets the position where the left mouse button was last pressed on the given control.
        /// </summary>
        /// <param name="owner">The control.</param>
        /// <param name="point">The position, relative to the control.</param>
        public static void SetOnMouseLeftButtonDown(Control owner, Point point)
        {
            owner.SetValue(OnMouseLeftButtonDownProperty, point);
        }

        #endregion

        #region Attached Property: OnMouseRightButtonDownProperty


        /// <summary>
        /// Identifies the OnMouseRightButtonDown attached property: the position, relative to the control,
        /// where the right mouse button was last pressed. Defaults to (-1, -1).
        /// </summary>
        public static readonly AttachedProperty<Point> OnMouseRightButtonDownProperty = AvaloniaProperty.RegisterAttached<Control, Point>(
                             "OnMouseRightButtonDown",
                             typeof(MouseManager),
                             new Point(-1, -1),
                             defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// Gets the position where the right mouse button was last pressed on the given control.
        /// </summary>
        /// <param name="owner">The control.</param>
        /// <returns>The position, relative to the control.</returns>
        public static Point GetOnMouseRightButtonDown(Control owner)
        {
            return owner.GetValue(OnMouseRightButtonDownProperty);
        }

        /// <summary>
        /// Sets the position where the right mouse button was last pressed on the given control.
        /// </summary>
        /// <param name="owner">The control.</param>
        /// <param name="point">The position, relative to the control.</param>
        public static void SetOnMouseRightButtonDown(Control owner, Point point)
        {
            owner.SetValue(OnMouseRightButtonDownProperty, point);
        }

        #endregion

        #region Attached Property: OnMouseDoubleClickProperty


        /// <summary>
        /// Identifies the OnMouseDoubleClick attached property: the position, relative to the control,
        /// of the last double click. Defaults to (-1, -1).
        /// </summary>
        public static readonly AttachedProperty<Point> OnMouseDoubleClickProperty = AvaloniaProperty.RegisterAttached<Control, Point>(
                             "OnMouseDoubleClick",
                             typeof(MouseManager),
                             new Point(-1, -1),
                             defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// Gets the position of the last double click on the given control.
        /// </summary>
        /// <param name="owner">The control.</param>
        /// <returns>The position, relative to the control.</returns>
        public static Point GetOnMouseDoubleClick(Control owner)
        {
            return owner.GetValue(OnMouseDoubleClickProperty);
        }

        /// <summary>
        /// Sets the position of the last double click on the given control.
        /// </summary>
        /// <param name="owner">The control.</param>
        /// <param name="point">The position, relative to the control.</param>
        public static void SetOnMouseDoubleClick(Control owner, Point point)
        {
            owner.SetValue(OnMouseDoubleClickProperty, point);
        }

        #endregion

        #region Attached Property: OnContextMenuOpeningProperty

        /// <summary>
        /// Identifies the OnContextMenuOpening attached property: the position, relative to the control,
        /// where the context menu was last requested. Defaults to (-1, -1).
        /// </summary>
        public static readonly AttachedProperty<Point> OnContextMenuOpeningProperty = AvaloniaProperty.RegisterAttached<Control, Point>(
                             "OnContextMenuOpening",
                             typeof(MouseManager),
                             new Point(-1, -1),
                             defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// Gets the position where the context menu was last requested on the given control.
        /// </summary>
        /// <param name="owner">The control.</param>
        /// <returns>The position, relative to the control.</returns>
        public static Point GetOnContextMenuOpening(Control owner)
        {
            return owner.GetValue(OnContextMenuOpeningProperty);
        }

        /// <summary>
        /// Sets the position where the context menu was last requested on the given control.
        /// </summary>
        /// <param name="owner">The control.</param>
        /// <param name="point">The position, relative to the control.</param>
        public static void SetOnContextMenuOpening(Control owner, Point point)
        {
            owner.SetValue(OnContextMenuOpeningProperty, point);
        }

        #endregion

    }
}
