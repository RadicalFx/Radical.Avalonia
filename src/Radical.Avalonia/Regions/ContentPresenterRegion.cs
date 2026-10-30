using Radical.Conversions;
using Radical.Avalonia.ComponentModel;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;

namespace Radical.Avalonia.Regions
{
    /// <summary>
    /// An content region hosted by a ContentPresenter.
    /// </summary>
    public sealed class ContentPresenterRegion : ContentRegion<ContentPresenter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPresenterRegion"/> class.
        /// </summary>
        public ContentPresenterRegion()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPresenterRegion"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ContentPresenterRegion(string name )
            : base( name )
        {

        }

        /// <summary>
        /// Called to get content.
        /// </summary>
        /// <returns>The current content.</returns>
        protected override AvaloniaObject OnGetContent()
        {
            return Element.Content as AvaloniaObject;
        }

        /// <summary>
        /// Called to set new content.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="args">The cancel even arguments.</param>
        protected override void OnSetContent( AvaloniaObject view, CancelEventArgs args )
        {
            if (Element.Content is AvaloniaObject previous)
            {
                (RegionService.Conventions.GetViewDataContext(previous, RegionService.Conventions.DefaultViewDataContextSearchBehavior) as IExpectViewClosingCallback)?.OnViewClosing(args);
            }

            if ( !args.Cancel )
            {
                Element.Content = view;
            }
        }
    }
}
