using Radical.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;
using System.Windows.Input;

namespace Radical.Avalonia.Input
{
    /// <summary>
    /// Extension methods for <see cref="ICommand"/>.
    /// </summary>
    public static class CommandExtensions
    {
        /// <summary>
        /// Gets the key gestures associated with the given command.
        /// </summary>
        /// <param name="source">The command, must be a <see cref="DelegateCommand"/>.</param>
        /// <returns>The key gestures, or an empty list if there are none.</returns>
        /// <exception cref="NotSupportedException">The command is not a <see cref="DelegateCommand"/>.</exception>
        public static IEnumerable<KeyGesture> GetGestures(this ICommand source)
        {
            Ensure.That(source).Named("source").IsNotNull();

            IEnumerable<KeyGesture> gestures = null;

            if (source is DelegateCommand cmd)
            {
                if (cmd.InputBindings != null && cmd.InputBindings.Count > 0)
                {
                    gestures = cmd.InputBindings.Select(ib => ib.Gesture);
                }
            }
            else
            {
                throw new NotSupportedException(string.Format("Unsupported command type: {0}", source));
            }

            return gestures ?? new KeyGesture[0];
        }
    }
}
