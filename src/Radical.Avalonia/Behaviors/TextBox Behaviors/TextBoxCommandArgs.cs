using System;

namespace Radical.Avalonia.Behaviors
{
    /// <summary>
    /// The parameter passed to text box commands, describing the pressed key.
    /// </summary>
    public class TextBoxCommandArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextBoxCommandArgs"/> class.
        /// </summary>
        /// <param name="key">The pressed key.</param>
        /// <param name="modifiers">The pressed key modifiers.</param>
        /// <param name="commandParameter">The command parameter configured on the text box.</param>
        public TextBoxCommandArgs(global::Avalonia.Input.Key key, global::Avalonia.Input.KeyModifiers modifiers, object commandParameter)
        {
            Key = key;
            Modifiers = modifiers;
            CommandParameter = commandParameter;
        }

        /// <summary>
        /// Gets the pressed key.
        /// </summary>
        public global::Avalonia.Input.Key Key { get; private set; }
        /// <summary>
        /// Gets the pressed key modifiers.
        /// </summary>
        public global::Avalonia.Input.KeyModifiers Modifiers { get; private set; }

        /// <summary>
        /// Gets the command parameter configured on the text box.
        /// </summary>
        public object CommandParameter { get; private set; }
    }
}
