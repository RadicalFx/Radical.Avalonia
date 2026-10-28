using Radical.Avalonia.ComponentModel;
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace Radical.Avalonia.Regions
{
    /// <summary>
    /// Region extensions.
    /// </summary>
    public static class RegionExtensions
    {
        /// <summary>
        /// Loads the given content async in the supplied region.
        /// </summary>
        /// <param name="region">The region</param>
        /// <param name="viewFactory">the view factory</param>
        /// <param name="millisecondsDelay">The async load delay.</param>
        public static void SetContentAsync( this IContentRegion region, Func<AvaloniaObject> viewFactory, int millisecondsDelay = 2000 )
        {
            DispatcherTimer.RunOnce( () =>
                {
                    var view = viewFactory();
                    region.Content = view;
                }, TimeSpan.FromMilliseconds( millisecondsDelay ) );
        }

        /// <summary>
        /// Add the given content async to the supplied region.
        /// </summary>
        /// <param name="region">the region.</param>
        /// <param name="viewFactory">the view factory</param>
        /// <param name="millisecondsDelay">The async load delay.</param>
        public static void AddContentAsync( this IElementsRegion region, Func<AvaloniaObject> viewFactory, int millisecondsDelay = 2000 )
        {
            DispatcherTimer.RunOnce( () =>
                {
                    var view = viewFactory();
                    region.Add( view );
                }, TimeSpan.FromMilliseconds( millisecondsDelay ) );
        }
    }
}
