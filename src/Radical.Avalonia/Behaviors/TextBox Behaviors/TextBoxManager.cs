using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace Radical.Avalonia.Behaviors
{
    public static class TextBoxManager
    {
        static readonly EventHandler<KeyEventArgs> onPreviewKeyDown;
        static readonly EventHandler<FocusChangedEventArgs> onGotFocus;

        static TextBoxManager()
        {
            onPreviewKeyDown = (s, e) =>
            {
                var d = (AvaloniaObject)s;
                TextBoxCommandKeyHandler.Handle(GetCommand(d), GetCommandParameter(d), e);
            };

            onGotFocus = (s, e) =>
            {
                var source = (s as TextBox);

                /*
                 * A pointer press moves the caret after the TextBox
                 * got focus, clearing the selection: select the text
                 * once the current input event has been processed.
                 */
                Dispatcher.UIThread.Post(() => source.SelectAll());
            };

            CommandProperty.Changed.AddClassHandler<AvaloniaObject, ICommand>(OnCommandChanged);
            AutoSelectTextProperty.Changed.AddClassHandler<TextBox, bool>(OnAutoSelectTextChanged);
        }

        #region Attached Property: Command

        public static readonly AttachedProperty<ICommand> CommandProperty = AvaloniaProperty.RegisterAttached<AvaloniaObject, ICommand>(
                                      "Command",
                                      typeof(TextBoxManager));


        public static ICommand GetCommand(AvaloniaObject owner)
        {
            return owner.GetValue(CommandProperty);
        }

        public static void SetCommand(AvaloniaObject owner, ICommand value)
        {
            owner.SetValue(CommandProperty, value);
        }

        #endregion

        #region Attached Property: CommandParameter

        public static readonly AttachedProperty<object> CommandParameterProperty = AvaloniaProperty.RegisterAttached<AvaloniaObject, object>(
                                      "CommandParameter",
                                      typeof(TextBoxManager));


        public static object GetCommandParameter(AvaloniaObject owner)
        {
            return owner.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(AvaloniaObject owner, object value)
        {
            owner.SetValue(CommandParameterProperty, value);
        }

        #endregion

        private static void OnCommandChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs<ICommand> e)
        {
            if (!DesignTimeHelper.GetIsInDesignMode())
            {
                if (d is TextBox textBox)
                {
                    textBox.RemoveHandler(InputElement.KeyDownEvent, onPreviewKeyDown);

                    if (e.NewValue.GetValueOrDefault() != null)
                    {
                        textBox.AddHandler(InputElement.KeyDownEvent, onPreviewKeyDown, RoutingStrategies.Tunnel);
                    }
                }
            }
        }

        #region Attached Property: AutoSelectText

        public static readonly AttachedProperty<bool> AutoSelectTextProperty = AvaloniaProperty.RegisterAttached<TextBox, bool>(
                                      "AutoSelectText",
                                      typeof(TextBoxManager));


        public static bool GetAutoSelectText(TextBox owner)
        {
            return owner.GetValue(AutoSelectTextProperty);
        }

        public static void SetAutoSelectText(TextBox owner, bool value)
        {
            owner.SetValue(AutoSelectTextProperty, value);
        }

        #endregion

        private static void OnAutoSelectTextChanged(TextBox source, AvaloniaPropertyChangedEventArgs<bool> e)
        {
            source.GotFocus -= onGotFocus;

            if (e.NewValue.GetValueOrDefault())
            {
                source.GotFocus += onGotFocus;
            }
        }
    }
}
