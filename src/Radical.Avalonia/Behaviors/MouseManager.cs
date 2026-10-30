using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Radical.Avalonia.Behaviors
{
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


        public static readonly AttachedProperty<Point> OnMouseLeftButtonDownProperty = AvaloniaProperty.RegisterAttached<Control, Point>(
                             "OnMouseLeftButtonDown",
                             typeof(MouseManager),
                             new Point(-1, -1),
                             defaultBindingMode: BindingMode.TwoWay);

        public static Point GetOnMouseLeftButtonDown(Control owner)
        {
            return owner.GetValue(OnMouseLeftButtonDownProperty);
        }

        public static void SetOnMouseLeftButtonDown(Control owner, Point point)
        {
            owner.SetValue(OnMouseLeftButtonDownProperty, point);
        }

        #endregion

        #region Attached Property: OnMouseRightButtonDownProperty


        public static readonly AttachedProperty<Point> OnMouseRightButtonDownProperty = AvaloniaProperty.RegisterAttached<Control, Point>(
                             "OnMouseRightButtonDown",
                             typeof(MouseManager),
                             new Point(-1, -1),
                             defaultBindingMode: BindingMode.TwoWay);

        public static Point GetOnMouseRightButtonDown(Control owner)
        {
            return owner.GetValue(OnMouseRightButtonDownProperty);
        }

        public static void SetOnMouseRightButtonDown(Control owner, Point point)
        {
            owner.SetValue(OnMouseRightButtonDownProperty, point);
        }

        #endregion

        #region Attached Property: OnMouseDoubleClickProperty


        public static readonly AttachedProperty<Point> OnMouseDoubleClickProperty = AvaloniaProperty.RegisterAttached<Control, Point>(
                             "OnMouseDoubleClick",
                             typeof(MouseManager),
                             new Point(-1, -1),
                             defaultBindingMode: BindingMode.TwoWay);

        public static Point GetOnMouseDoubleClick(Control owner)
        {
            return owner.GetValue(OnMouseDoubleClickProperty);
        }

        public static void SetOnMouseDoubleClick(Control owner, Point point)
        {
            owner.SetValue(OnMouseDoubleClickProperty, point);
        }

        #endregion

        #region Attached Property: OnContextMenuOpeningProperty

        public static readonly AttachedProperty<Point> OnContextMenuOpeningProperty = AvaloniaProperty.RegisterAttached<Control, Point>(
                             "OnContextMenuOpening",
                             typeof(MouseManager),
                             new Point(-1, -1),
                             defaultBindingMode: BindingMode.TwoWay);

        public static Point GetOnContextMenuOpening(Control owner)
        {
            return owner.GetValue(OnContextMenuOpeningProperty);
        }

        public static void SetOnContextMenuOpening(Control owner, Point point)
        {
            owner.SetValue(OnContextMenuOpeningProperty, point);
        }

        #endregion

    }
}
