using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;

namespace Radical.Avalonia.Behaviors
{
    public class ScrollViewerManager
    {
        static ScrollViewerManager()
        {
            HorizontalOffsetProperty.Changed.AddClassHandler<ScrollViewer, double>(OnHorizontalOffsetChanged);
            VerticalOffsetProperty.Changed.AddClassHandler<ScrollViewer, double>(OnVerticalOffsetChanged);
        }

        #region Attached Property: IsSetup

        static readonly AttachedProperty<bool> IsSetupProperty = AvaloniaProperty.RegisterAttached<ScrollViewer, bool>(
                            "IsSetup",
                            typeof(ScrollViewerManager));

        static void EnsureSetup(ScrollViewer viewer)
        {
            if (!viewer.GetValue(IsSetupProperty))
            {
                viewer.ScrollChanged += OnSourceScrollChanged;
                viewer.Loaded += OnViewerLoaded;
                viewer.SetValue(IsSetupProperty, true);
            }
        }

        /*
         * Before the first layout pass the ScrollViewer has no extent and
         * coerces any offset to zero: offsets set before the viewer is loaded,
         * e.g. by a binding, are applied once it has been laid out.
         */
        private static void OnViewerLoaded(object sender, RoutedEventArgs e)
        {
            var viewer = (ScrollViewer)sender;
            var offset = viewer.Offset;

            var horizontalOffset = GetHorizontalOffset(viewer);
            if (horizontalOffset >= 0)
            {
                offset = offset.WithX(horizontalOffset);
            }

            var verticalOffset = GetVerticalOffset(viewer);
            if (verticalOffset >= 0)
            {
                offset = offset.WithY(verticalOffset);
            }

            viewer.Offset = offset;
        }

        private static void OnSourceScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var viewer = (ScrollViewer)sender;
            if (!viewer.IsLoaded)
            {
                // the first layout pass would overwrite offsets not applied yet
                return;
            }

            SetHorizontalOffset(viewer, viewer.Offset.X);
            SetVerticalOffset(viewer, viewer.Offset.Y);
        }

        #endregion

        #region Attached Property: HorizontalOffsetProperty

        public static readonly AttachedProperty<double> HorizontalOffsetProperty = AvaloniaProperty.RegisterAttached<ScrollViewer, double>(
                            "HorizontalOffset",
                            typeof(ScrollViewerManager),
                            -1,
                            defaultBindingMode: BindingMode.TwoWay);

        public static double GetHorizontalOffset(ScrollViewer owner)
        {
            return owner.GetValue(HorizontalOffsetProperty);
        }

        public static void SetHorizontalOffset(ScrollViewer owner, double value)
        {
            owner.SetValue(HorizontalOffsetProperty, value);
        }

        private static void OnHorizontalOffsetChanged(ScrollViewer viewer, AvaloniaPropertyChangedEventArgs<double> e)
        {
            EnsureSetup(viewer);

            if (viewer.IsLoaded)
            {
                var offset = Math.Max(0, e.NewValue.GetValueOrDefault());
                viewer.Offset = viewer.Offset.WithX(offset);
            }
        }

        #endregion

        #region Attached Property: VerticalOffsetProperty

        public static readonly AttachedProperty<double> VerticalOffsetProperty = AvaloniaProperty.RegisterAttached<ScrollViewer, double>(
                            "VerticalOffset",
                            typeof(ScrollViewerManager),
                            -1,
                            defaultBindingMode: BindingMode.TwoWay);

        public static double GetVerticalOffset(ScrollViewer owner)
        {
            return owner.GetValue(VerticalOffsetProperty);
        }

        public static void SetVerticalOffset(ScrollViewer owner, double value)
        {
            owner.SetValue(VerticalOffsetProperty, value);
        }

        private static void OnVerticalOffsetChanged(ScrollViewer viewer, AvaloniaPropertyChangedEventArgs<double> e)
        {
            EnsureSetup(viewer);

            if (viewer.IsLoaded)
            {
                var offset = Math.Max(0, e.NewValue.GetValueOrDefault());
                viewer.Offset = viewer.Offset.WithY(offset);
            }
        }

        #endregion

    }
}
