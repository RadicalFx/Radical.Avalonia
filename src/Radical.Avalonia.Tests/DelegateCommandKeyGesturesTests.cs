using Avalonia.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Avalonia.CommandBuilders;
using Radical.Avalonia.ComponentModel;
using Radical.Avalonia.Input;
using System.Linq;

namespace Radical.Avalonia.Tests
{
    [TestClass]
    public class DelegateCommandKeyGesturesTests
    {
        class ViewModel
        {
            [KeyBinding(Key.S, Modifiers = KeyModifiers.Control)]
            public void Save() { }
        }

        [AvaloniaTestMethod]
        public void AddKeyGesture_should_create_key_bindings_bound_to_the_command()
        {
            var command = DelegateCommand.Create()
                .AddKeyGesture(Key.F5)
                .AddKeyGesture(Key.S, KeyModifiers.Control | KeyModifiers.Shift);

            Assert.AreEqual(2, command.InputBindings.Count);
            Assert.IsTrue(command.InputBindings.All(kb => kb.Command == command));
            CollectionAssert.AreEqual(
                new[] { new KeyGesture(Key.F5), new KeyGesture(Key.S, KeyModifiers.Control | KeyModifiers.Shift) },
                command.GetGestures().ToList());
        }

        [AvaloniaTestMethod]
        public void Command_created_by_builder_should_honor_KeyBinding_attributes()
        {
            var builder = new DelegateCommandBuilder();
            builder.TryGenerateCommandData("Save", new ViewModel(), out CommandData data);

            var command = builder.CreateCommand(data);

            CollectionAssert.AreEqual(new[] { new KeyGesture(Key.S, KeyModifiers.Control) }, command.GetGestures().ToList());
        }
    }
}
