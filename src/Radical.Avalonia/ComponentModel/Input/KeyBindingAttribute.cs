using Avalonia.Input;
using System;

namespace Radical.Avalonia.ComponentModel
{
    /// <summary>
    /// Defines a key binding for a command generated from the decorated method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class KeyBindingAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyBindingAttribute"/> class.
        /// </summary>
        /// <param name="key">The keyboard key.</param>
        public KeyBindingAttribute(Key key)
        {
            Key = key;
        }

        /// <summary>
        /// Gets the keyboard key to associate with the command.
        /// </summary>
        /// <value>The keyboard key.</value>
        public Key Key { get; private set; }

        /// <summary>
        /// Gets the modifier keyboard key to associate with the command.
        /// </summary>
        /// <value>The modifier keyboard key.</value>
        public KeyModifiers Modifiers { get; set; }
    }
}
