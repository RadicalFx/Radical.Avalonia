using Avalonia.Controls;

namespace Radical.Avalonia.Behaviors
{
    public static class DesignTimeHelper
    {
        /// <summary>
        /// Gets a value indicating whether we are in design mode.
        /// </summary>
        /// <returns><c>True</c> if we are in design mode, otherwise <c>false</c>.</returns>
        public static bool GetIsInDesignMode()
        {
            var isInDesignMode = Design.IsDesignMode;

            return isInDesignMode;
        }
    }
}
