using System;

namespace Radical.Avalonia.Behaviors
{
    public class TextBoxCommandArgs : EventArgs
    {
        public TextBoxCommandArgs(global::Avalonia.Input.Key key, global::Avalonia.Input.KeyModifiers modifiers, object commandParameter)
        {
            Key = key;
            Modifiers = modifiers;
            CommandParameter = commandParameter;
        }

        public global::Avalonia.Input.Key Key { get; private set; }
        public global::Avalonia.Input.KeyModifiers Modifiers { get; private set; }

        public object CommandParameter { get; private set; }
    }
}
