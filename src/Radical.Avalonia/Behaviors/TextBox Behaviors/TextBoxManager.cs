using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace Radical.Avalonia.Behaviors
{
    /// <summary>
    /// Exposes attached properties to execute commands on key press and to
    /// automatically select the text of <see cref="TextBox"/> controls.
    /// </summary>
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

        /// <summary>
        /// Identifies the Command attached property: the command executed, with a <see cref="TextBoxCommandArgs"/>
        /// parameter, if the pressed key matches one of the command gestures, or is Enter when the command has no gestures.
        /// </summary>
        public static readonly AttachedProperty<ICommand> CommandProperty = AvaloniaProperty.RegisterAttached<AvaloniaObject, ICommand>(
                                      "Command",
                                      typeof(TextBoxManager));


        /// <summary>
        /// Gets the command of the given text box.
        /// </summary>
        /// <param name="owner">The text box.</param>
        /// <returns>The command.</returns>
        public static ICommand GetCommand(AvaloniaObject owner)
        {
            return owner.GetValue(CommandProperty);
        }

        /// <summary>
        /// Sets the command of the given text box.
        /// </summary>
        /// <param name="owner">The text box.</param>
        /// <param name="value">The command.</param>
        public static void SetCommand(AvaloniaObject owner, ICommand value)
        {
            owner.SetValue(CommandProperty, value);
        }

        #endregion

        #region Attached Property: CommandParameter

        /// <summary>
        /// Identifies the CommandParameter attached property: the parameter passed to the command, wrapped in a <see cref="TextBoxCommandArgs"/>.
        /// </summary>
        public static readonly AttachedProperty<object> CommandParameterProperty = AvaloniaProperty.RegisterAttached<AvaloniaObject, object>(
                                      "CommandParameter",
                                      typeof(TextBoxManager));


        /// <summary>
        /// Gets the command parameter of the given text box.
        /// </summary>
        /// <param name="owner">The text box.</param>
        /// <returns>The command parameter.</returns>
        public static object GetCommandParameter(AvaloniaObject owner)
        {
            return owner.GetValue(CommandParameterProperty);
        }

        /// <summary>
        /// Sets the command parameter of the given text box.
        /// </summary>
        /// <param name="owner">The text box.</param>
        /// <param name="value">The command parameter.</param>
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

        /// <summary>
        /// Identifies the AutoSelectText attached property: when <c>true</c> the whole text is selected when the text box gets focus.
        /// </summary>
        public static readonly AttachedProperty<bool> AutoSelectTextProperty = AvaloniaProperty.RegisterAttached<TextBox, bool>(
                                      "AutoSelectText",
                                      typeof(TextBoxManager));


        /// <summary>
        /// Gets a value indicating whether the text of the given text box is selected when it gets focus.
        /// </summary>
        /// <param name="owner">The text box.</param>
        /// <returns><c>true</c> if the text is automatically selected; otherwise, <c>false</c>.</returns>
        public static bool GetAutoSelectText(TextBox owner)
        {
            return owner.GetValue(AutoSelectTextProperty);
        }

        /// <summary>
        /// Sets a value indicating whether the text of the given text box is selected when it gets focus.
        /// </summary>
        /// <param name="owner">The text box.</param>
        /// <param name="value"><c>true</c> to automatically select the text; otherwise, <c>false</c>.</param>
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
