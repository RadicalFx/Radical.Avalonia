using Avalonia.Xaml.Interactivity;
using Radical.ComponentModel.Messaging;
using Radical.Conversions;
using Radical.Linq;
using Radical.Reflection;
using Radical.Avalonia.Behaviors;
using Radical.Avalonia.Bootstrap;
using Radical.Avalonia.ComponentModel;
using Radical.Avalonia.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace Radical.Avalonia.Services
{
    /// <summary>
    /// Handles Presentation conventions.
    /// </summary>
    class ConventionsHandler : IConventionsHandler
    {
        readonly IMessageBroker broker;
        readonly IReleaseComponents releaser;
        readonly bool releaserSupportsDispose = false;
        readonly BootstrapConventions bootstrapConventions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionsHandler"/> class.
        /// </summary>
        /// <param name="broker">The broker.</param>
        /// <param name="releaser">The releaser.</param>
        /// <param name="bootstrapConventions">The Bootstrap Conventions</param>
        public ConventionsHandler(IMessageBroker broker, IReleaseComponents releaser, BootstrapConventions bootstrapConventions)
        {
            this.broker = broker;
            this.releaser = releaser;
            releaserSupportsDispose = releaser.GetType().IsAttributeDefined<SupportComponentDisposeAttribute>();

            this.bootstrapConventions = bootstrapConventions;

            DefaultResolveViewModelType = viewType =>
            {
                var aName = new AssemblyName(viewType.Assembly.FullName);
                var vmTypeName = string.Format("{0}.{1}Model, {2}", viewType.Namespace, viewType.Name, aName.FullName);
                var vmType = Type.GetType(vmTypeName, false);

                return vmType;
            };

            ResolveViewModelType = viewType =>
            {
                return DefaultResolveViewModelType(viewType);
            };

            DefaultResolveViewType = viewModelType =>
            {
                var aName = new AssemblyName(viewModelType.Assembly.FullName);
                var vTypeName = string.Format("{0}.{1}, {2}", viewModelType.Namespace, viewModelType.Name.Remove(viewModelType.Name.LastIndexOf('M')), aName.FullName);
                var vType = Type.GetType(vTypeName, true);

                return vType;
            };

            ResolveViewType = viewModelType =>
            {
                return DefaultResolveViewType(viewModelType);
            };

            DefaultViewReleaseHandler = (view, behavior) =>
            {
                var autoDispose = view.GetType().IsAttributeDefined<ViewManualReleaseAttribute>() == false;
                if(autoDispose || behavior == ViewReleaseBehavior.Force)
                {
                    var vm = GetViewDataContext(view, ViewDataContextSearchBehavior.LocalOnly);
                    if(vm != null)
                    {
                        SetViewDataContext(view, null);
                        if(ShouldUnsubscribeViewModelOnRelease(view))
                        {
                            this.broker.Unsubscribe(vm);
                        }

                        this.releaser.Release(vm);
                        if( !releaserSupportsDispose && vm is IDisposable)
                        {
                            ((IDisposable)vm).Dispose();
                        }
                    }

                    DetachViewBehaviors(view);

                    this.releaser.Release(view);
                    if(!releaserSupportsDispose && view is IDisposable)
                    {
                        ((IDisposable)view).Dispose();
                    }
                }
            };

            ViewReleaseHandler = (view, behavior) =>
            {
                DefaultViewReleaseHandler(view, behavior);
            };

            bool isSingletonView(AvaloniaObject view)
            {
                var implementation = view.GetType();
                var contracts = this.bootstrapConventions.SelectViewContracts(implementation);
                var isShell = this.bootstrapConventions.IsShellView(contracts, implementation);

                return isShell;
            }

            DefaultShouldUnsubscribeViewModelOnRelease = view => !isSingletonView(view);

            ShouldUnsubscribeViewModelOnRelease = view => DefaultShouldUnsubscribeViewModelOnRelease(view);

            DefaultShouldReleaseView = view => !isSingletonView(view);

            ShouldReleaseView = view => DefaultShouldReleaseView(view);

            DefaultShouldUnregisterRegionManagerOfView = view => !isSingletonView(view);

            ShouldUnregisterRegionManagerOfView = view => DefaultShouldUnregisterRegionManagerOfView(view);

            DefaultFindHostingWindowOf = vm =>
            {
                var view = GetViewOfViewModel(vm);
                var window = FindWindowOf(view);//.FindWindow();
                return window;
            };

            FindHostingWindowOf = vm =>
            {
                return DefaultFindHostingWindowOf(vm);
            };

            DefaultFindWindowOf = dependencyObject =>
            {
                // Like WPF Window.GetWindow, rely on the logical tree first so that
                // windows not shown yet are found; fall back to the visual root.
                var window = (dependencyObject as ILogical)?.FindLogicalAncestorOfType<Window>(includeSelf: true);
                if(window == null && dependencyObject is Visual visual)
                {
                    window = TopLevel.GetTopLevel(visual) as Window;
                }
                return window;
            };

            FindWindowOf = dependencyObject =>
            {
                return DefaultFindWindowOf(dependencyObject);
            };

            DefaultViewHasDataContext = (view, behavior) =>
            {
                return GetViewDataContext(view, behavior) != null;
            };

            ViewHasDataContext = (view, behavior) =>
            {
                return DefaultViewHasDataContext(view, behavior);
            };

            DefaultGetViewDataContext = (view, behavior) =>
            {
                if(view is StyledElement element)
                {
                    if(behavior == ViewDataContextSearchBehavior.Legacy)
                    {
                        return element.DataContext;
                    }
                    else if(element.IsSet(StyledElement.DataContextProperty))
                    {
                        return element.DataContext;
                    }
                }

                return null;
            };

            GetViewDataContext = (view, behavior) =>
            {
                return DefaultGetViewDataContext(view, behavior);
            };

            DefaultSetViewDataContext = (view, dc) =>
            {
                if(view is StyledElement element)
                {
                    element.DataContext = dc;
                }
            };

            SetViewDataContext = (view, dc) =>
            {
                DefaultSetViewDataContext(view, dc);
            };

            DefaultShouldExposeViewModelAsStaticResource = (view, viewModel) =>
            {
                return viewModel.GetType().GetCustomAttribute<ExposeViewModelAsStaticResourceAttribute>() != null;
            };

            ShouldExposeViewModelAsStaticResource = (view, viewModel) =>
            {
                return DefaultShouldExposeViewModelAsStaticResource(view, viewModel);
            };

            DefaultExposeViewModelAsStaticResource = (view, viewModel) =>
            {
                var key = GenerateViewModelStaticResourceKey(viewModel);
                if(view is StyledElement element)
                {
                    element.Resources.Add(key, viewModel);
                }
            };

            ExposeViewModelAsStaticResource = (view, dc) =>
            {
                DefaultExposeViewModelAsStaticResource(view, dc);
            };

            DefaultGenerateViewModelStaticResourceKey = (dc) =>
            {
                return dc.GetType().Name;
            };

            GenerateViewModelStaticResourceKey = (dc) =>
            {
                return DefaultGenerateViewModelStaticResourceKey(dc);
            };

            DefaultTryHookClosedEventOfHostOf = (view, closedCallback) =>
            {
                //TODO: dobbiamo anche cercare una IClosableView oltre che una Window
                var window = FindWindowOf(view);
                if(window != null)
                {
                    void closed(object s, EventArgs e)
                    {
                        try
                        {
                            closedCallback(window);
                        }
                        finally
                        {
                            window.Closed -= closed;
                        }
                    }

                    window.Closed += closed;
                }

                return window;
            };

            TryHookClosedEventOfHostOf = (view, closedCallback) =>
            {
                return DefaultTryHookClosedEventOfHostOf(view, closedCallback);
            };

            DefaultIsHostingView = fe => fe.GetType().Name.EndsWith("View");

            IsHostingView = fe => DefaultIsHostingView(fe);

            DefaultAttachViewToViewModel = (view, viewModel) =>
            {
                if (viewModel is IViewModel ivm)
                {
                    ivm.View = view;
                }
            };

            AttachViewToViewModel = (view, viewModel) =>
            {
                DefaultAttachViewToViewModel(view, viewModel);
            };

            DefaultGetViewOfViewModel = viewModel =>
            {
                if(viewModel is IViewModel)
                {
                    return ((IViewModel)viewModel).View;
                }

                return null;
            };

            GetViewOfViewModel = viewModel =>
            {
                return DefaultGetViewOfViewModel(viewModel);
            };

            DefaultAttachViewBehaviors = view =>
            {
                var bhv = Interaction.GetBehaviors(view);
                if(view is Window && bhv.OfType<WindowLifecycleNotificationsBehavior>().None())
                {
                    bhv.Add(new WindowLifecycleNotificationsBehavior(this.broker, this));
                }
                else if(view is Control && bhv.OfType<FrameworkElementLifecycleNotificationsBehavior>().None())
                {
                    bhv.Add(new FrameworkElementLifecycleNotificationsBehavior(this.broker, this));
                }

                if(bhv.OfType<DependencyObjectCloseHandlerBehavior>().None())
                {
                    bhv.Add(new DependencyObjectCloseHandlerBehavior(this.broker, this));
                }
            };

            AttachViewBehaviors = view =>
            {
                DefaultAttachViewBehaviors(view);
            };

            DefaultDetachViewBehaviors = view =>
            {
                var bhv = Interaction.GetBehaviors(view);
                var toRemove = bhv.OfType<DependencyObjectCloseHandlerBehavior>().Cast<AvaloniaObject>().ToList();
                if(view is Window)
                {
                    toRemove.AddRange(bhv.OfType<WindowLifecycleNotificationsBehavior>());
                }
                else if(view is Control)
                {
                    toRemove.AddRange(bhv.OfType<FrameworkElementLifecycleNotificationsBehavior>());
                }

                RemoveBehaviors(bhv, toRemove);
            };

            DetachViewBehaviors = view =>
            {
                DefaultDetachViewBehaviors(view);
            };

            DefaultShouldNotifyViewModelLoaded = (view, dataContext) =>
            {
                if(dataContext == null)
                {
                    return false;
                }

                var hasAttribute = dataContext.GetType().IsAttributeDefined<NotifyLoadedAttribute>();
                var hasRegions = RegionService.CurrentService.HoldsRegionManager(view);

                return hasAttribute || hasRegions;
            };

            ShouldNotifyViewModelLoaded = (view, dataContext) =>
            {
                return DefaultShouldNotifyViewModelLoaded(view, dataContext);
            };

            DefaultShouldNotifyViewLoaded = view =>
            {
                /*
                 * we should decide if the attribute must be applied on the view or, as in this fix,
                 * mainly for backward compatibility, can be applied also on the ViewModel and the
                 * _View_Loaded message is still broadcasted.
                 */
                //var dataContext = this.GetViewDataContext( view );
                //var hasAttributeOnViewModel = dataContext != null && dataContext.GetType().IsAttributeDefined<NotifyLoadedAttribute>();
                var hasAttributeOnView = view.GetType().IsAttributeDefined<NotifyLoadedAttribute>();
                var hasRegions = RegionService.CurrentService.HoldsRegionManager(view);

                return /* hasAttributeOnViewModel || */ hasAttributeOnView || hasRegions;
            };

            ShouldNotifyViewLoaded = view =>
            {
                return DefaultShouldNotifyViewLoaded(view);
            };

            DefaultGenerateServiceStaticResourceKey = type => type.Namespace + "." + type.Name;
            GenerateServiceStaticResourceKey = type => DefaultGenerateServiceStaticResourceKey(type);
        }

        static void RemoveBehaviors(BehaviorCollection behaviors, List<AvaloniaObject> toRemove)
        {
            if(toRemove.Count == 0)
            {
                return;
            }

            var isAttached = behaviors.OfType<Behavior>().Any(b => b.AssociatedObject != null);
            if(isAttached)
            {
                toRemove.ForEach(x => behaviors.Remove(x));
            }
            else
            {
                // Xaml.Behaviors throws when removing a behavior from a collection whose
                // control has left the visual tree (e.g. a view released after its window
                // closed): clearing and re-adding the remaining behaviors is safe instead.
                var remaining = behaviors.Except(toRemove).ToList();
                behaviors.Clear();
                behaviors.AddRange(remaining);
            }
        }

        /// <summary>
        /// Gets or sets the view model type resolver that can resolve the view model type given the view type.
        /// </summary>
        /// <value>
        /// The view model type resolver.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<Type, Type> ResolveViewModelType { get; set; }

        /// <summary>
        /// Default: Gets or sets the view model type resolver that can resolve the view model type given the view type.
        /// </summary>
        /// <value>
        /// The view model type resolver.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<Type, Type> DefaultResolveViewModelType { get; private set; }

        /// <summary>
        /// Gets or sets the view type resolver that can resolve the view type given the view model type.
        /// </summary>
        /// <value>
        /// The view type resolver.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<Type, Type> ResolveViewType { get; set; }

        /// <summary>
        /// Default: Gets or sets the view type resolver that can resolve the view type given the view model type.
        /// </summary>
        /// <value>
        /// The view type resolver.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<Type, Type> DefaultResolveViewType { get; private set; }

        /// <summary>
        /// Gets or sets the window finder.
        /// </summary>
        /// <value>
        /// The window finder.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<object, Window> FindHostingWindowOf { get; set; }

        /// <summary>
        /// Default: Gets or sets the window finder.
        /// </summary>
        /// <value>
        /// The window finder.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<object, Window> DefaultFindHostingWindowOf { get; set; }

        /// <summary>
        /// Gets or sets the View -&gt; window finder, that given a View finds the root hosting Window for the given View.
        /// </summary>
        /// <value>
        /// The find window of.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<AvaloniaObject, Window> FindWindowOf { get; set; }

        /// <summary>
        /// Default: Gets or sets the View -&gt; window finder, that given a View finds the root hosting Window for the given View.
        /// </summary>
        /// <value>
        /// The find window of.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<AvaloniaObject, Window> DefaultFindWindowOf { get; private set; }

        /// <summary>
        /// Gets or sets the logic that determines if view has data context.
        /// </summary>
        /// <value>
        /// The logic that determines if view has data context.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<AvaloniaObject, ViewDataContextSearchBehavior, bool> ViewHasDataContext { get; set; }

        /// <summary>
        /// Default: Gets or sets the logic that determines if view has data context.
        /// </summary>
        /// <value>
        /// The logic that determines if view has data context.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<AvaloniaObject, ViewDataContextSearchBehavior, bool> DefaultViewHasDataContext { get; private set; }

        /// <summary>
        /// Gets or sets the logic that sets the view data context.
        /// </summary>
        /// <value>
        /// The logic that sets the view data context.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Action<AvaloniaObject, object> SetViewDataContext { get; set; }

        [IgnorePropertyInjectionAttribue]
        public Func<AvaloniaObject, object, bool> DefaultShouldExposeViewModelAsStaticResource { get; private set; }

        [IgnorePropertyInjectionAttribue]
        public Func<AvaloniaObject, object, bool> ShouldExposeViewModelAsStaticResource { get; set; }

        [IgnorePropertyInjectionAttribue]
        public Action<AvaloniaObject, object> DefaultExposeViewModelAsStaticResource { get; private set; }

        [IgnorePropertyInjectionAttribue]
        public Action<AvaloniaObject, object> ExposeViewModelAsStaticResource { get; set; }

        [IgnorePropertyInjectionAttribue]
        public Func<object, string> DefaultGenerateViewModelStaticResourceKey { get; private set; }

        [IgnorePropertyInjectionAttribue]
        public Func<object, string> GenerateViewModelStaticResourceKey { get; set; }

        /// <summary>
        /// Default: Gets or sets the logic that sets the view data context.
        /// </summary>
        /// <value>
        /// The logic that sets the view data context.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Action<AvaloniaObject, object> DefaultSetViewDataContext { get; private set; }

        /// <summary>
        /// Gets or sets the logic that gets view data context.
        /// </summary>
        /// <value>
        /// The logic that gets view data context.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<AvaloniaObject, ViewDataContextSearchBehavior, object> GetViewDataContext { get; set; }

        /// <summary>
        /// Default: Gets or sets the logic that gets view data context.
        /// </summary>
        /// <value>
        /// The logic that gets view data context.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<AvaloniaObject, ViewDataContextSearchBehavior, object> DefaultGetViewDataContext { get; private set; }

        /// <summary>
        /// Tries to hook closed event of an the element in the visual tree that hosts this given view.
        /// If the hook succeeded the given callback will be called once the hosting element is closed.
        /// </summary>
        /// <returns>
        /// The element, that supports closed notifications, in the visual tree that hosts the given view; otherwise <c>null</c>.
        ///   </returns>
        [IgnorePropertyInjectionAttribue]
        public Func<AvaloniaObject, Action<AvaloniaObject>, AvaloniaObject> TryHookClosedEventOfHostOf { get; set; }

        /// <summary>
        /// Default: Tries to hook closed event of an the element in the visual tree that hosts this given view.
        /// If the hook succeeded the given callback will be called once the hosting element is closed.
        /// </summary>
        [IgnorePropertyInjectionAttribue]
        public Func<AvaloniaObject, Action<AvaloniaObject>, AvaloniaObject> DefaultTryHookClosedEventOfHostOf { get; private set; }


        /// <summary>
        /// Gets or sets the convention that determines if the given Control is a hosting view.
        /// </summary>
        /// <value>
        /// The convention that determines if the given Control is a hosting view.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<Control, bool> IsHostingView { get; set; }

        /// <summary>
        /// Default: Gets or sets the convention that determines if the given Control is a hosting view.
        /// </summary>
        /// <value>
        /// The convention that determines if the given Control is a hosting view.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<Control, bool> DefaultIsHostingView { get; private set; }

        /// <summary>
        /// Gets or sets the attach view to view model handler.
        /// </summary>
        /// <value>
        /// The attach view to view model handler.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Action<AvaloniaObject, object> AttachViewToViewModel { get; set; }

        /// <summary>
        /// Default: Gets or sets the attach view to view model handler.
        /// </summary>
        /// <value>
        /// The attach view to view model handler.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Action<AvaloniaObject, object> DefaultAttachViewToViewModel { get; private set; }

        /// <summary>
        /// Gets the view of the given view model.
        /// </summary>
        /// <value>
        /// The get view of view model handler.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<object, AvaloniaObject> GetViewOfViewModel { get; set; }

        /// <summary>
        /// Default: Gets the view of the given view model.
        /// </summary>
        /// <value>
        /// The get view of view model handler.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<object, AvaloniaObject> DefaultGetViewOfViewModel { get; private set; }

        /// <summary>
        /// Gets an opportunity to attach behaviors to the view.
        /// </summary>
        /// <value>
        /// The attach view behaviors handler.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Action<AvaloniaObject> AttachViewBehaviors { get; set; }

        /// <summary>
        /// Default: Gets an opportunity to attach behaviors to the view.
        /// </summary>
        /// <value>
        /// The attach view behaviors handler.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Action<AvaloniaObject> DefaultAttachViewBehaviors { get; private set; }

        /// <summary>
        /// Gets an opportunity to detach behaviors from the view.
        /// </summary>
        /// <value>
        /// The detach view behaviors handler.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Action<AvaloniaObject> DetachViewBehaviors { get; set; }

        /// <summary>
        /// Default: Gets an opportunity to detach behaviors from the view.
        /// </summary>
        /// <value>
        /// The detach view behaviors handler.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Action<AvaloniaObject> DefaultDetachViewBehaviors { get; private set; }

        /// <summary>
        /// Gets or sets the logic that determines if ViewModel should notify the loaded message.
        /// </summary>
        /// <value>
        /// The logic that determines if ViewModel should notify the loaded message.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<AvaloniaObject, object, bool> ShouldNotifyViewModelLoaded { get; set; }

        /// <summary>
        /// Default: Gets or sets the logic that determines if ViewModel should notify the loaded message.
        /// </summary>
        /// <value>
        /// The logic that determines if ViewModel should notify the loaded message.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<AvaloniaObject, object, bool> DefaultShouldNotifyViewModelLoaded { get; private set; }

        /// <summary>
        /// Gets or sets the logic that determines if View should notify the loaded message.
        /// </summary>
        /// <value>
        /// The logic that determines if View should notify the loaded message.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<AvaloniaObject, bool> ShouldNotifyViewLoaded { get; set; }

        /// <summary>
        /// Default: Gets or sets the logic that determines if View should notify the loaded message.
        /// </summary>
        /// <value>
        /// The logic that determines if View should notify the loaded message.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<AvaloniaObject, bool> DefaultShouldNotifyViewLoaded { get; private set; }

        /// <summary>
        /// Gets or sets the view release handler that is responsible to release views and view models.
        /// </summary>
        /// <value>
        /// The view release handler.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Action<AvaloniaObject, ViewReleaseBehavior> ViewReleaseHandler { get; set; }

        /// <summary>
        /// Default: Gets or sets the view release handler that is responsible to release views and view models.
        /// </summary>
        /// <value>
        /// The view release handler.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Action<AvaloniaObject, ViewReleaseBehavior> DefaultViewReleaseHandler { get; private set; }


        /// <summary>
        /// Gets or sets the handler that determines if a region manager for the given view should be un-registered, the default behavior is that the region manager should be realsed if the view is not a singleton view.
        /// </summary>
        /// <value>
        /// The un-register region manager handler.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<AvaloniaObject, bool> ShouldUnregisterRegionManagerOfView { get; set; }

        /// <summary>
        /// Default: Gets or sets the handler that determines if a region manager for the given view should be un-registered, the default behavior is that the region manager should be realsed if the view is not a singleton view.
        /// </summary>
        /// <value>
        /// The un-register region manager handler.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<AvaloniaObject, bool> DefaultShouldUnregisterRegionManagerOfView { get; private set; }

        /// <summary>
        /// Gets or sets the handler that determines if a view should be released, the default behavior is that the view is released if not a singleton view.
        /// </summary>
        /// <value>
        /// The view release handler.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<AvaloniaObject, bool> ShouldReleaseView { get; set; }

        /// <summary>
        /// Default: Gets or sets the handler that determines if a view should be released, the default behavior is that the view is released if not a singleton view.
        /// </summary>
        /// <value>
        /// The view release handler.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<AvaloniaObject, bool> DefaultShouldReleaseView { get; private set; }

        /// <summary>
        /// Gets or sets the handler that determines if a view model should be automatically unsubscribed from all the subscriptions when its view is relased, the default behavior is that the view model is unsubscribed if the view is not a singleton view.
        /// </summary>
        /// <value>
        /// The unsubscribe handler.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<AvaloniaObject, bool> ShouldUnsubscribeViewModelOnRelease { get; set; }

        /// <summary>
        /// Default: Gets or sets the handler that determines if a view model should be automatically unsubscribed from all the subscriptions when its view is relased, the default behavior is that the view model is unsubscribed if the view is not a singleton view.
        /// </summary>
        /// <value>
        /// The unsubscribe handler.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<AvaloniaObject, bool> DefaultShouldUnsubscribeViewModelOnRelease { get; private set; }

        /// <summary>
        /// Gets or sets the default view data context search behavior.
        /// </summary>
        /// <value>
        /// The default view data context search behavior.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public ViewDataContextSearchBehavior DefaultViewDataContextSearchBehavior{ get; set; }

        /// <summary>
        /// Gets or sets the generate service static resource key convention.
        /// </summary>
        /// <value>
        /// The generate service static resource key convention.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<Type, object> GenerateServiceStaticResourceKey { get; set; }

        /// <summary>
        /// Default: Gets or sets the generate service static resource key convention.
        /// </summary>
        /// <value>
        /// The default generate service static resource key.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<Type, object> DefaultGenerateServiceStaticResourceKey { get; }
    }
}
