using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace Radical.Avalonia.Behaviors
{
    /// <summary>
    /// The DragDropManager class exposing all the drag 'n' drop related attached properties.
    /// </summary>
    public static class DragDropManager
    {
        static readonly ConcurrentDictionary<string, DataFormat<object>> dataFormats = new ConcurrentDictionary<string, DataFormat<object>>();

        /// <summary>
        /// Gets the in-process data format used to drag objects whose
        /// <see cref="DataObjectTypeProperty"/> is the given value.
        /// </summary>
        /// <param name="dataObjectType">The data object type.</param>
        /// <returns>The data format.</returns>
        public static DataFormat<object> GetDataFormat(string dataObjectType)
        {
            return dataFormats.GetOrAdd(dataObjectType, t => DataFormat.CreateInProcessFormat<object>(t));
        }

        /// <summary>
        /// Gets the in-process data format used to drag objects of the
        /// given type, when no <see cref="DataObjectTypeProperty"/> is set.
        /// </summary>
        /// <param name="type">The type of the dragged objects.</param>
        /// <returns>The data format.</returns>
        public static DataFormat<object> GetDataFormat(Type type)
        {
            return GetDataFormat(type.FullName);
        }

        /// <summary>
        /// Starts the platform drag operation, replaceable by tests
        /// as the headless platform has no drag source.
        /// </summary>
        internal static Func<PointerPressedEventArgs, IDataTransfer, DragDropEffects, Task<DragDropEffects>> doDragDropAsync = DragDrop.DoDragDropAsync;

        static DragDropManager()
        {
            DataObjectProperty.Changed.AddClassHandler<Control>(OnDataObjectChanged);
            OnDropCommandProperty.Changed.AddClassHandler<Control>(OnOnDropCommandChanged);
        }

        #region Attached Property: IsDragSourceAttached

        static readonly AttachedProperty<bool> IsDragSourceAttachedProperty = AvaloniaProperty.RegisterAttached<Control, bool>(
                                      "IsDragSourceAttached",
                                      typeof(DragDropManager));

        #endregion

        #region Attached Property: IsDropTargetAttached

        static readonly AttachedProperty<bool> IsDropTargetAttachedProperty = AvaloniaProperty.RegisterAttached<Control, bool>(
                                      "IsDropTargetAttached",
                                      typeof(DragDropManager));

        #endregion

        #region Attached Property: DropTarget

        /// <summary>
        /// Identifies the DropTarget attached property: the object passed to the drop
        /// commands as the target of the drag and drop operation.
        /// </summary>
        public static readonly AttachedProperty<object> DropTargetProperty = AvaloniaProperty.RegisterAttached<Control, object>(
                                      "DropTarget",
                                      typeof(DragDropManager));


        /// <summary>
        /// Gets the drop target of the given control.
        /// </summary>
        /// <param name="owner">The control.</param>
        /// <returns>The drop target.</returns>
        public static object GetDropTarget(Control owner)
        {
            return owner.GetValue(DropTargetProperty);
        }

        /// <summary>
        /// Sets the drop target of the given control.
        /// </summary>
        /// <param name="owner">The control.</param>
        /// <param name="value">The drop target.</param>
        public static void SetDropTarget(Control owner, object value)
        {
            owner.SetValue(DropTargetProperty, value);
        }

        #endregion

        #region Attached Property: DataObjectType

        /// <summary>
        /// Identifies the DataObjectType attached property: the name of the data format used
        /// to drag the data object. When not set the full name of the data object type is used.
        /// </summary>
        public static readonly AttachedProperty<string> DataObjectTypeProperty = AvaloniaProperty.RegisterAttached<Control, string>(
                                      "DataObjectType",
                                      typeof(DragDropManager));


        /// <summary>
        /// Gets the data object type of the given control.
        /// </summary>
        /// <param name="owner">The control.</param>
        /// <returns>The data object type.</returns>
        public static string GetDataObjectType(Control owner)
        {
            return owner.GetValue(DataObjectTypeProperty);
        }

        /// <summary>
        /// Sets the data object type of the given control.
        /// </summary>
        /// <param name="owner">The control.</param>
        /// <param name="value">The data object type.</param>
        public static void SetDataObjectType(Control owner, string value)
        {
            owner.SetValue(DataObjectTypeProperty, value);
        }

        #endregion

        #region Attached Property: DataObject

        /// <summary>
        /// Identifies the DataObject attached property: the object dragged when a drag
        /// operation starts from the control. Setting it makes the control a drag source.
        /// </summary>
        public static readonly AttachedProperty<object> DataObjectProperty = AvaloniaProperty.RegisterAttached<Control, object>(
                                      "DataObject",
                                      typeof(DragDropManager));


        /// <summary>
        /// Gets the data object of the given control.
        /// </summary>
        /// <param name="owner">The control.</param>
        /// <returns>The data object.</returns>
        public static object GetDataObject(Control owner)
        {
            return owner.GetValue(DataObjectProperty);
        }

        /// <summary>
        /// Sets the data object of the given control.
        /// </summary>
        /// <param name="owner">The control.</param>
        /// <param name="value">The data object.</param>
        public static void SetDataObject(Control owner, object value)
        {
            owner.SetValue(DataObjectProperty, value);
        }

        #endregion

        private static void OnDataObjectChanged(Control d, AvaloniaPropertyChangedEventArgs e)
        {
            var attached = d.GetValue(IsDragSourceAttachedProperty);
            if (!attached)
            {
                d.SetValue(IsDragSourceAttachedProperty, true);

                d.AddHandler(InputElement.PointerPressedEvent, OnDragSourcePointerPressed, RoutingStrategies.Tunnel);
                d.AddHandler(InputElement.PointerMovedEvent, OnDragSourcePointerMoved, handledEventsToo: true);
                d.AddHandler(InputElement.PointerReleasedEvent, OnDragSourcePointerReleased, handledEventsToo: true);
            }
        }

        static void OnDragSourcePointerPressed(object sender, PointerPressedEventArgs args)
        {
            var d = (Control)sender;
            if (args.GetCurrentPoint(d).Properties.IsLeftButtonPressed)
            {
                /*
                 * Avalonia starts a drag operation from the event
                 * of the pointer press that initiated it.
                 */
                d.SetValue(PressedEventArgsProperty, args);
                d.SetValue(StartPointProperty, args.GetPosition(null));
            }
        }

        static void OnDragSourcePointerReleased(object sender, PointerReleasedEventArgs args)
        {
            ((Control)sender).SetValue(PressedEventArgsProperty, null);
        }

        static void OnDragSourcePointerMoved(object sender, PointerEventArgs args)
        {
            var d = (Control)sender;
            var pressedEventArgs = d.GetValue(PressedEventArgsProperty);
            var isDragging = GetIsDragging(d);

            if (pressedEventArgs != null && args.GetCurrentPoint(d).Properties.IsLeftButtonPressed && !isDragging)
            {
                var position = args.GetPosition(null);
                var startPoint = d.GetValue(StartPointProperty);
                var dragDistance = GetMinimumDragDistance(d);

                if (Math.Abs(position.X - startPoint.X) > dragDistance.Width ||
                    Math.Abs(position.Y - startPoint.Y) > dragDistance.Height)
                {
                    d.SetValue(PressedEventArgsProperty, null);
                    StartDrag(d, pressedEventArgs);
                }
            }
        }

        static Size GetMinimumDragDistance(Control d)
        {
            var settings = d.GetPlatformSettings();
            return settings?.GetTapSize(PointerType.Mouse) ?? new Size(4, 4);
        }

        static async void StartDrag(Control d, PointerPressedEventArgs e)
        {
            var sourceItem = FindDragContainer(e.Source as Visual) ?? d;

            SetIsDragging(d, true);

            try
            {
                var obj = GetDataObject(sourceItem);
                var objType = GetDataObjectType(sourceItem);

                var format = string.IsNullOrEmpty(objType)
                    ? GetDataFormat(obj.GetType())
                    : GetDataFormat(objType);

                var data = new DataTransfer();
                data.Add(DataTransferItem.Create(format, obj));

                await doDragDropAsync(e, data, DragDropEffects.Move);
            }
            finally
            {
                SetIsDragging(d, false);
            }
        }

        #region Attached Property: IsDragging

        /// <summary>
        /// Identifies the read-only IsDragging attached property: <c>true</c> while a drag
        /// operation started from the control is in progress.
        /// </summary>
        public static readonly AttachedProperty<bool> IsDraggingProperty = AvaloniaProperty.RegisterAttached<Control, bool>(
                                      "IsDragging",
                                      typeof(DragDropManager));


        /// <summary>
        /// Gets the is dragging.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <returns><c>true</c> if a drag operation started from the control is in progress; otherwise, <c>false</c>.</returns>
        public static bool GetIsDragging(Control owner)
        {
            return owner.GetValue(IsDraggingProperty);
        }

        static void SetIsDragging(Control owner, bool value)
        {
            owner.SetValue(IsDraggingProperty, value);
        }

        #endregion

        #region Attached Property: StartPoint

        static readonly AttachedProperty<Point> StartPointProperty = AvaloniaProperty.RegisterAttached<Control, Point>(
                                      "StartPoint",
                                      typeof(DragDropManager));

        static readonly AttachedProperty<PointerPressedEventArgs> PressedEventArgsProperty = AvaloniaProperty.RegisterAttached<Control, PointerPressedEventArgs>(
                                      "PressedEventArgs",
                                      typeof(DragDropManager));

        #endregion

        #region Attached Property: OnDropCommand

        /// <summary>
        /// Identifies the OnDropCommand attached property: the command executed, with a
        /// <see cref="DropArgs"/> parameter, when data is dropped on the control. Setting it makes the control a drop target.
        /// </summary>
        public static readonly AttachedProperty<ICommand> OnDropCommandProperty = AvaloniaProperty.RegisterAttached<Control, ICommand>(
                                      "OnDropCommand",
                                      typeof(DragDropManager));


        /// <summary>
        /// Gets the drop command of the given control.
        /// </summary>
        /// <param name="owner">The control.</param>
        /// <returns>The drop command.</returns>
        public static ICommand GetOnDropCommand(Control owner)
        {
            return owner.GetValue(OnDropCommandProperty);
        }

        /// <summary>
        /// Sets the drop command of the given control.
        /// </summary>
        /// <param name="owner">The control.</param>
        /// <param name="value">The drop command.</param>
        public static void SetOnDropCommand(Control owner, ICommand value)
        {
            owner.SetValue(OnDropCommandProperty, value);
        }

        #endregion

        #region Attached Property: OnDragEnterCommand

        /// <summary>
        /// Identifies the OnDragEnterCommand attached property: the command executed, with a
        /// <see cref="DragEnterArgs"/> parameter, when a drag operation enters the control.
        /// </summary>
        public static readonly AttachedProperty<ICommand> OnDragEnterCommandProperty = AvaloniaProperty.RegisterAttached<Control, ICommand>(
                                      "OnDragEnterCommand",
                                      typeof(DragDropManager));


        /// <summary>
        /// Gets the drag enter command of the given control.
        /// </summary>
        /// <param name="owner">The control.</param>
        /// <returns>The drag enter command.</returns>
        public static ICommand GetOnDragEnterCommand(Control owner)
        {
            return owner.GetValue(OnDragEnterCommandProperty);
        }

        /// <summary>
        /// Sets the drag enter command of the given control.
        /// </summary>
        /// <param name="owner">The control.</param>
        /// <param name="value">The drag enter command.</param>
        public static void SetOnDragEnterCommand(Control owner, ICommand value)
        {
            owner.SetValue(OnDragEnterCommandProperty, value);
        }

        #endregion

        #region Attached Property: OnDragLeaveCommand

        /// <summary>
        /// Identifies the OnDragLeaveCommand attached property: the command executed, with a
        /// <see cref="DragLeaveArgs"/> parameter, when a drag operation leaves the control.
        /// </summary>
        public static readonly AttachedProperty<ICommand> OnDragLeaveCommandProperty = AvaloniaProperty.RegisterAttached<Control, ICommand>(
                                      "OnDragLeaveCommand",
                                      typeof(DragDropManager));


        /// <summary>
        /// Gets the drag leave command of the given control.
        /// </summary>
        /// <param name="owner">The control.</param>
        /// <returns>The drag leave command.</returns>
        public static ICommand GetOnDragLeaveCommand(Control owner)
        {
            return owner.GetValue(OnDragLeaveCommandProperty);
        }

        /// <summary>
        /// Sets the drag leave command of the given control.
        /// </summary>
        /// <param name="owner">The control.</param>
        /// <param name="value">The drag leave command.</param>
        public static void SetOnDragLeaveCommand(Control owner, ICommand value)
        {
            owner.SetValue(OnDragLeaveCommandProperty, value);
        }

        #endregion

        /// <summary>
        /// Finds, starting from the given element and walking up the visual tree,
        /// the first control matching the given condition.
        /// </summary>
        static Control FindSelfOrParent(Visual originalSource, Func<Control, bool> matchCondition)
        {
            return originalSource?
                .GetSelfAndVisualAncestors()
                .OfType<Control>()
                .FirstOrDefault(matchCondition);
        }

        /// <summary>
        /// Finds the drag container that is the element
        /// that holds the DataObject that will be dragged.
        /// </summary>
        /// <param name="originalSource">
        /// The original source where the
        /// drag operation started.
        /// </param>
        /// <returns>
        /// The element that holds the DataObject,
        /// or null if none can be found.
        /// </returns>
        static Control FindDragContainer(Visual originalSource)
        {
            return FindSelfOrParent(originalSource, t => GetDataObject(t) != null);
        }

        static object FindDropTarget(Visual originalSource)
        {
            var element = FindSelfOrParent(originalSource, t => GetDropTarget(t) != null);
            if (element != null)
            {
                return GetDropTarget(element);
            }

            return null;
        }

        static ICommand FindDropCommand(Visual originalSource)
        {
            var element = FindSelfOrParent(originalSource, t => GetOnDropCommand(t) != null);
            if (element != null)
            {
                return GetOnDropCommand(element);
            }

            return null;
        }

        static ICommand FindDragEnterCommand(Visual originalSource)
        {
            var element = FindSelfOrParent(originalSource, t => GetOnDragEnterCommand(t) != null);
            if (element != null)
            {
                return GetOnDragEnterCommand(element);
            }

            return null;
        }

        static ICommand FindDragLeaveCommand(Visual originalSource)
        {
            var element = FindSelfOrParent(originalSource, t => GetOnDragLeaveCommand(t) != null);
            if (element != null)
            {
                return GetOnDragLeaveCommand(element);
            }

            return null;
        }

        private static void OnOnDropCommandChanged(Control ui, AvaloniaPropertyChangedEventArgs e)
        {
            var attached = ui.GetValue(IsDropTargetAttachedProperty);
            if (!attached)
            {
                ui.SetValue(IsDropTargetAttachedProperty, true);

                DragDrop.SetAllowDrop(ui, true);

                // Elements with no background are not hit test visible, thus cannot be drop targets.
                var backgroundProperty = ui switch
                {
                    TemplatedControl => TemplatedControl.BackgroundProperty,
                    Panel => Panel.BackgroundProperty,
                    Border => Border.BackgroundProperty,
                    _ => null
                };

                if (backgroundProperty != null && ui.GetValue(backgroundProperty) == null)
                {
                    ui.SetValue(backgroundProperty, Brushes.Transparent);
                }

                ui.AddHandler(DragDrop.DragEnterEvent, OnDragEnter);
                ui.AddHandler(DragDrop.DragLeaveEvent, OnDragLeave);
                ui.AddHandler(DragDrop.DragOverEvent, OnDragOver);
                ui.AddHandler(DragDrop.DropEvent, OnDrop);
            }
        }

        static void OnDragEnter(object sender, DragEventArgs args)
        {
            var os = args.Source as Visual;
            var command = FindDragEnterCommand(os);
            if (command != null)
            {
                var dropTarget = FindDropTarget(os);
                var cmdArgs = new DragEnterArgs(
                    args.DataTransfer,
                    args.KeyModifiers,
                    dropTarget,
                    args.DragEffects);

                if (command.CanExecute(cmdArgs))
                {
                    command.Execute(cmdArgs);
                }
            }
        }

        static void OnDragLeave(object sender, DragEventArgs args)
        {
            var os = args.Source as Visual;
            var command = FindDragLeaveCommand(os);
            if (command != null)
            {
                var dropTarget = FindDropTarget(os);
                var cmdArgs = new DragLeaveArgs(
                    args.DataTransfer,
                    args.KeyModifiers,
                    dropTarget,
                    args.DragEffects);

                if (command.CanExecute(cmdArgs))
                {
                    command.Execute(cmdArgs);
                }
            }
        }

        static void OnDragOver(object sender, DragEventArgs args)
        {
            var os = args.Source as Visual;

            var command = FindDropCommand(os);
            if (command != null)
            {
                var dropTarget = FindDropTarget(os);

                var position = os != null ? args.GetPosition(os) : new Point(0, 0);

                var cmdArgs = new DragOverArgs(
                    args.DataTransfer,
                    args.KeyModifiers,
                    dropTarget,
                    args.DragEffects,
                    position);

                var result = command.CanExecute(cmdArgs);
                if (!result)
                {
                    args.DragEffects = cmdArgs.Effects;
                    args.Handled = true;
                }
            }
            else
            {
                // the allowed effects, as set by the drag source
                args.Handled = true;
            }
        }

        static void OnDrop(object sender, DragEventArgs args)
        {
            var os = args.Source as Visual;

            var command = FindDropCommand(os);
            if (command != null)
            {
                var dropTarget = FindDropTarget(os);

                var position = os != null ? args.GetPosition(os) : new Point(0, 0);

                var cmdArgs = new DropArgs(args.DataTransfer, args.KeyModifiers, dropTarget, position);
                command.Execute(cmdArgs);
            }
        }
    }
}
