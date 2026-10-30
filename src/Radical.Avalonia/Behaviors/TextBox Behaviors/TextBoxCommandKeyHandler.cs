using Radical.Linq;
using Radical.Avalonia.Input;
using System.Linq;
using System.Windows.Input;
using Avalonia.Input;

namespace Radical.Avalonia.Behaviors
{
    static class TextBoxCommandKeyHandler
    {
        /// <summary>
        /// Executes the command, passing in a <see cref="TextBoxCommandArgs"/>, if the pressed
        /// key matches one of the command gestures, or is Enter when the command has no gestures.
        /// </summary>
        /// <param name="cmd">The command to execute.</param>
        /// <param name="prm">The command parameter.</param>
        /// <param name="e">The key event data.</param>
        public static void Handle(ICommand cmd, object prm, KeyEventArgs e)
        {
            if (cmd != null && cmd.CanExecute(prm))
            {
                var gestures = cmd.GetGestures();
                var senderGestures = gestures.Where(gesture => gesture.Matches(e));

                if (((gestures.None() && e.Key == Key.Enter) || senderGestures.Any()))
                {
                    var k = e.Key;
                    var m = KeyModifiers.None;

                    var gesture = senderGestures.FirstOrDefault();
                    if (gesture != null)
                    {
                        k = gesture.Key;
                        m = gesture.KeyModifiers;
                    }

                    var args = new TextBoxCommandArgs(k, m, prm);
                    cmd.Execute(args);
                    e.Handled = true;
                }
            }
        }
    }
}
