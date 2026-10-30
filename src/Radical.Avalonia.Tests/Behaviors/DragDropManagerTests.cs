using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Media;
using Avalonia.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Avalonia.Behaviors;
using Radical.Avalonia.Input;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Radical.Avalonia.Tests.Behaviors
{
    [TestClass]
    public class DragDropManagerTests
    {
        class Person
        {
        }

        /// <summary>
        /// The headless platform has no drag source: records drag operations instead.
        /// </summary>
        class RecordingDragSource
        {
            public List<(IDataTransfer Data, DragDropEffects AllowedEffects, bool WasDragging)> Operations { get; } = new();

            public Control Source { get; set; }

            public Task<DragDropEffects> DoDragDropAsync(PointerPressedEventArgs triggerEvent, IDataTransfer dataTransfer, DragDropEffects allowedEffects)
            {
                Operations.Add((dataTransfer, allowedEffects, DragDropManager.GetIsDragging(Source)));
                return Task.FromResult(DragDropEffects.Move);
            }
        }

        RecordingDragSource dragSource;
        Window window;
        Border source;
        Border target;
        List<object> executions;
        List<object> canExecuteRequests;

        [TestInitialize]
        public void Setup()
        {
            executions = new List<object>();
            canExecuteRequests = new List<object>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            DragDropManager.doDragDropAsync = DragDrop.DoDragDropAsync;
        }

        void Show(object dataObject, string dataObjectType = null, bool canDrop = true)
        {
            source = new Border { Height = 50, Background = Brushes.White };
            DragDropManager.SetDataObject(source, dataObject);
            DragDropManager.SetDataObjectType(source, dataObjectType);

            target = new Border { Height = 50 };
            DragDropManager.SetDropTarget(target, "the target");
            DragDropManager.SetOnDropCommand(target, DelegateCommand.Create()
                .OnCanExecute(p => { canExecuteRequests.Add(p); return canDrop; })
                .OnExecute(p => executions.Add(p)));
            DragDropManager.SetOnDragEnterCommand(target, DelegateCommand.Create().OnExecute(p => executions.Add(p)));
            DragDropManager.SetOnDragLeaveCommand(target, DelegateCommand.Create().OnExecute(p => executions.Add(p)));

            dragSource = new RecordingDragSource { Source = source };
            DragDropManager.doDragDropAsync = dragSource.DoDragDropAsync;

            window = new Window { Width = 300, Height = 300, Content = new StackPanel { Children = { source, target } } };
            window.Show();
            Dispatcher.UIThread.RunJobs();
        }

        void Drag(Point from, Point to)
        {
            window.MouseDown(from, MouseButton.Left);
            window.MouseMove(to, RawInputModifiers.LeftMouseButton);
            window.MouseUp(to, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaTestMethod, TestCategory("Behaviors")]
        public void Dragging_a_source_should_start_a_drag_operation_with_the_data_object()
        {
            var person = new Person();
            Show(person);

            Drag(new Point(10, 10), new Point(10, 30));

            var (data, allowedEffects, wasDragging) = dragSource.Operations.Single();
            Assert.AreSame(person, data.TryGetValue(DragDropManager.GetDataFormat(typeof(Person))));
            Assert.AreEqual(DragDropEffects.Move, allowedEffects);
            Assert.IsTrue(wasDragging);
            Assert.IsFalse(DragDropManager.GetIsDragging(source));
        }

        [AvaloniaTestMethod, TestCategory("Behaviors")]
        public void Moving_less_than_the_drag_distance_should_not_start_a_drag_operation()
        {
            Show(new Person());

            Drag(new Point(10, 10), new Point(11, 11));

            Assert.AreEqual(0, dragSource.Operations.Count);
        }

        [AvaloniaTestMethod, TestCategory("Behaviors")]
        public void Dropping_should_execute_the_drop_command_with_the_dragged_object()
        {
            var person = new Person();
            Show(person, dataObjectType: "people");
            Drag(new Point(10, 10), new Point(10, 30));
            var data = dragSource.Operations.Single().Data;

            window.DragDrop(new Point(20, 70), RawDragEventType.DragEnter, data, DragDropEffects.Move, RawInputModifiers.Control);
            window.DragDrop(new Point(20, 70), RawDragEventType.Drop, data, DragDropEffects.Move, RawInputModifiers.Control);
            Dispatcher.UIThread.RunJobs();

            var drop = executions.OfType<DropArgs>().Single();
            Assert.AreSame(person, drop.GetData("people"));
            Assert.IsNull(drop.GetData(typeof(Person)));
            Assert.AreEqual("the target", drop.DropTarget);
            Assert.AreEqual(new Point(20, 20), drop.Position);
            Assert.AreEqual(KeyModifiers.Control, drop.KeyStates);
        }

        [AvaloniaTestMethod, TestCategory("Behaviors")]
        public void Drag_enter_over_and_leave_should_be_reported_with_the_allowed_effects()
        {
            Show(new Person());
            var data = new DataTransfer();
            data.Add(DataTransferItem.CreateText("text"));

            window.DragDrop(new Point(20, 70), RawDragEventType.DragEnter, data, DragDropEffects.Copy | DragDropEffects.Move, RawInputModifiers.None);
            window.DragDrop(new Point(20, 75), RawDragEventType.DragOver, data, DragDropEffects.Copy | DragDropEffects.Move, RawInputModifiers.None);
            window.DragDrop(new Point(20, 10), RawDragEventType.DragOver, data, DragDropEffects.Copy | DragDropEffects.Move, RawInputModifiers.None);
            Dispatcher.UIThread.RunJobs();

            var enter = executions.OfType<DragEnterArgs>().Single();
            Assert.AreEqual(DragDropEffects.Copy | DragDropEffects.Move, enter.AllowedEffects);
            Assert.AreEqual("the target", enter.DropTarget);

            var over = canExecuteRequests.OfType<DragOverArgs>().Single();
            Assert.AreEqual(DragDropEffects.Copy | DragDropEffects.Move, over.AllowedEffects);
            Assert.AreEqual(new Point(20, 25), over.Position);

            Assert.AreEqual(1, executions.OfType<DragLeaveArgs>().Count());
        }

        [AvaloniaTestMethod, TestCategory("Behaviors")]
        public void Drop_targets_should_allow_drop_and_be_hit_test_visible()
        {
            Show(new Person());

            Assert.IsTrue(DragDrop.GetAllowDrop(target));
            Assert.AreSame(Brushes.Transparent, target.Background);
            Assert.AreSame(Brushes.White, source.Background);
        }
    }
}
