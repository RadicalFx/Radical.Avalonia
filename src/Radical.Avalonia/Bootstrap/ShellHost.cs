using System;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace Radical.Avalonia.Bootstrap
{
    /// <summary>
    /// Shows the splash screen and the shell according to the application lifetime:
    /// windows on desktop, the main view on single-view platforms (mobile and browser).
    /// </summary>
    abstract class ShellHost
    {
        /// <summary>
        /// Creates the shell host for the given lifetime, replaceable by tests
        /// as single-view lifetimes cannot be implemented outside Avalonia.
        /// </summary>
        internal static Func<IApplicationLifetime, ShellHost> Factory = For;

        static ShellHost For(IApplicationLifetime lifetime)
        {
            return lifetime switch
            {
                IActivityApplicationLifetime activity => new ActivityShellHost(factory => activity.MainViewFactory = factory),
                ISingleViewApplicationLifetime singleView => new SingleViewShellHost(view => singleView.MainView = view),
                _ => new WindowShellHost(lifetime as IClassicDesktopStyleApplicationLifetime)
            };
        }

        public abstract void ShowSplashScreen(Control view, SplashScreenConfiguration configuration);

        /// <summary>
        /// Shows the shell, replacing the splash screen, if any.
        /// </summary>
        /// <param name="view">The shell view.</param>
        public abstract void ShowShell(Control view);

        protected static void EnsureIsNotWindow(Control view)
        {
            if (view is Window)
            {
                throw new NotSupportedException(string.Format(
                    "{0} is a Window. On single-view platforms (mobile and browser) the shell and the splash screen views must be controls, e.g. a UserControl.",
                    view.GetType().FullName));
            }
        }
    }

    class WindowShellHost(IClassicDesktopStyleApplicationLifetime desktop) : ShellHost
    {
        Window splashScreen;

        static Window AsWindow(Control view)
        {
            return view as Window ?? new Window() { Content = view };
        }

        public override void ShowSplashScreen(Control view, SplashScreenConfiguration configuration)
        {
            splashScreen = AsWindow(view);
            if (desktop != null)
            {
                desktop.MainWindow = splashScreen;
            }

            splashScreen.WindowStartupLocation = configuration.WindowStartupLocation;
            if (configuration.MinWidth.HasValue)
            {
                splashScreen.MinWidth = configuration.MinWidth.Value;
            }

            if (configuration.MinHeight.HasValue)
            {
                splashScreen.MinHeight = configuration.MinHeight.Value;
            }

            splashScreen.WindowDecorations = configuration.WindowDecorations;
            splashScreen.SizeToContent = configuration.SizeToContent;
            switch (splashScreen.SizeToContent)
            {
                case SizeToContent.Manual:
                    splashScreen.Width = configuration.Width;
                    splashScreen.Height = configuration.Height;
                    break;

                case SizeToContent.Height:
                    splashScreen.Width = configuration.Width;
                    break;

                case SizeToContent.Width:
                    splashScreen.Height = configuration.Height;
                    break;
            }

            splashScreen.Show();
        }

        public override void ShowShell(Control view)
        {
            var mainWindow = AsWindow(view);
            if (desktop != null)
            {
                desktop.MainWindow = mainWindow;
            }

            mainWindow.Show();

            splashScreen?.Close();
            splashScreen = null;
        }
    }

    class SingleViewShellHost(Action<Control> setMainView) : ShellHost
    {
        public override void ShowSplashScreen(Control view, SplashScreenConfiguration configuration)
        {
            EnsureIsNotWindow(view);
            setMainView(view);
        }

        public override void ShowShell(Control view)
        {
            EnsureIsNotWindow(view);
            setMainView(view);
        }
    }

    /// <summary>
    /// The activity lifetime creates the main view through a factory, invoked by the
    /// platform: the factory returns a host whose content is the current view, so that
    /// the shell can replace the splash screen once the main view has been created.
    /// </summary>
    class ActivityShellHost(Action<Func<Control>> setMainViewFactory) : ShellHost
    {
        ContentControl host;

        void Show(Control view)
        {
            EnsureIsNotWindow(view);

            if (host == null)
            {
                host = new ContentControl();
                setMainViewFactory(() => host);
            }

            host.Content = view;
        }

        public override void ShowSplashScreen(Control view, SplashScreenConfiguration configuration)
        {
            Show(view);
        }

        public override void ShowShell(Control view)
        {
            Show(view);
        }
    }
}
