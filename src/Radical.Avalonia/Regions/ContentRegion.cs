using Radical.Avalonia.ComponentModel;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;

namespace Radical.Avalonia.Regions
{
    /// <summary>
    /// A base abstract implementation of the <see cref="IContentRegion"/>.
    /// </summary>
    /// <typeparam name="T">The type of the element that hosts this region.</typeparam>
    public abstract class ContentRegion<T> :
        Region<T>,
        IContentRegion
        where T : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentRegion&lt;T&gt;"/> class.
        /// </summary>
        protected ContentRegion()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentRegion&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected ContentRegion(string name )
            : base( name )
        {

        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public override void Shutdown()
        {
            NotifyClosedAndEnsureRelease( Content );
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public AvaloniaObject Content
        {
            get { return OnGetContent(); }
            set
            {
                if ( value != Content )
                {
                    var args = new CancelEventArgs()
                    {
                        Cancel = false
                    };

                    var previous = Content;
                    OnSetContent( value, args );
                    if ( !args.Cancel )
                    {
                        OnContentSet( value, previous );
                    }
                }
            }
        }

        /// <summary>
        /// Called to get content.
        /// </summary>
        /// <returns>The current content.</returns>
        protected abstract AvaloniaObject OnGetContent();

        /// <summary>
        /// Called to set new content.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="args">The cancel even arguments.</param>
        protected abstract void OnSetContent( AvaloniaObject view, CancelEventArgs args );

        /// <summary>
        /// Called when content has been set.
        /// </summary>
        /// <param name="actual">The actual.</param>
        /// <param name="previous">The previous.</param>
        protected virtual void OnContentSet( AvaloniaObject actual, AvaloniaObject previous )
        {
            NotifyClosedAndEnsureRelease( previous );
        }
    }
}
