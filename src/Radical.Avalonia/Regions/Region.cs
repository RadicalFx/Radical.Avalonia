using Radical.Conversions;
using Radical.Diagnostics;
using Radical.Helpers;
using Radical.Reflection;
using Radical.Avalonia.ComponentModel;
using Radical.Avalonia.Internals;
using System;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.XamlIl.Runtime;
using Avalonia.Media;
using System.Collections.Generic;

namespace Radical.Avalonia.Regions
{
    /// <summary>
    /// A base abstract implementation of a region and it's relative markup extension.
    /// </summary>
    /// <typeparam name="T">The type of the element that hosts this region.</typeparam>
    public abstract class Region<T> :
        MarkupExtension,
        IRegion where T : Control
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly TraceSource Logger;

        IEnumerable<object> xamlParents;

        /// <summary>
        /// Initializes a new instance of the <see cref="Region&lt;T&gt;"/> class.
        /// </summary>
        protected Region()
        {
            Logger = new TraceSource( GetType().Name );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Region&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected Region(string name )
            : this()
        {
            Name = name;
        }

        /// <summary>
        /// Gets the view that hosts this region.
        /// </summary>
        public AvaloniaObject HostingView { get; private set; }

        /// <summary>
        /// Gets the name of the region.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// The number of milliseconds to wait before loading the region content.
        /// </summary>
        public int AsyncLoadDelay { get; set; }

        /// <summary>
        /// Gets the element.
        /// </summary>
        protected T Element
        {
            get;
            private set;
        }

        /// <summary>
        /// Called when the element is changed.
        /// </summary>
        protected virtual void OnElementChanged()
        {

        }

        /// <summary>
        /// When implemented in a derived class, returns an object that is set as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        /// <returns>
        /// The object value to set on the property where the extension is applied.
        /// </returns>
        public override sealed object ProvideValue( IServiceProvider serviceProvider )
        {
            if ( serviceProvider != null )
            {
                if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget service)
                {
                    if (!(service.TargetObject is T element))
                    {
                        //TODO: migliorare l'exception
                        var msg = string.Format("The TargetObject of this {0} is null.", GetType().ToString("SN"));
                        throw new NotSupportedException(msg);
                    }

                    if (Element != element)
                    {
                        Element = element;
                        OnElementChanged();
                    }

                    if (!Design.IsDesignMode)
                    {
                        // While loading XAML, Avalonia may not have attached the element
                        // to its parent yet: keep the loader's parents stack around so
                        // that the hosting view can still be found.
                        xamlParents = (serviceProvider.GetService(typeof(IAvaloniaXamlIlParentStackProvider)) as IAvaloniaXamlIlParentStackProvider)?.Parents;
                        AvaloniaObject view;
                        try
                        {
                            view = FindHostingViewOf(Element);
                        }
                        finally
                        {
                            xamlParents = null;
                        }

                        if (view == null)
                        {
                            //TODO: migliorare l'exception
                            var msg = string.Format("Cannot find any hosting view for this {0}.",
                                GetType().ToString("SN"));

                            throw new NotSupportedException(msg);
                        }

                        HostingView = view;

                        if (CommandLine.GetCurrent().Contains("hr"))
                        {
                            Logger.Warning("Regions highlighting is turned on.");

                            AdornerLayer.SetAdorner(Element, new RegionHilightAdorner(this, Brushes.Red));
                        }
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Closes this region, the close process is invoked by the host at close time.
        /// </summary>
        public abstract void Shutdown();

        /// <summary>
        /// Notify via IExpectViewClosedCallback and release the resources, even for nested views.
        /// </summary>
        /// <param name="view"></param>
        protected virtual void NotifyClosedAndEnsureRelease( AvaloniaObject view )
        {
            if ( RegionService.Conventions == null )
            {
                //BUG: se è null l'app sta facendo shutdown.
                return;
            }

            if ( view != null )
            {
                if ( RegionService.Conventions.ShouldUnregisterRegionManagerOfView( view ) && RegionService.CurrentService.HoldsRegionManager( view ) )
                {
                    RegionService.CurrentService.UnregisterRegionManager( view, UnregisterBehavior.WholeLogicalTreeChain );
                }

                if (RegionService.Conventions.GetViewDataContext( view, RegionService.Conventions.DefaultViewDataContextSearchBehavior ) is IExpectViewClosedCallback ievcc)
                {
                    ievcc.OnViewClosed();
                }

                if ( RegionService.Conventions.ShouldReleaseView( view ) )
                {
                    RegionService.Conventions.ViewReleaseHandler( view, ViewReleaseBehavior.Default );
                }
            }
        }

        /// <summary>
        /// Finds the hosting view of the given FramerowkElement.
        /// </summary>
        /// <param name="fe">The FramerowkElement for which to find the hosting view.</param>
        /// <returns></returns>
        protected virtual AvaloniaObject FindHostingViewOf( Control fe )
        {
            if ( fe == null )
            {
                return null;
            }
            else if ( RegionService.Conventions.IsHostingView( fe ) )
            {
                return fe;
            }
            else if ( fe.Parent != null )
            {
                return FindHostingViewOf( fe.Parent as Control );
            }

            return xamlParents?
                .OfType<Control>()
                .FirstOrDefault( p => p != fe && RegionService.Conventions.IsHostingView( p ) );
        }
    }
}
