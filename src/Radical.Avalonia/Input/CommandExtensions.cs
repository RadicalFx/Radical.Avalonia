using Radical.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;
using System.Windows.Input;

namespace Radical.Avalonia.Input
{
    public static class CommandExtensions
    {
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
