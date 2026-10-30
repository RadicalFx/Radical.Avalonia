using Avalonia.Controls;

namespace Radical.Avalonia.Tests.Boot.Presentation
{
    public class SingleViewSplashScreenView : UserControl
    {
        public static SingleViewSplashScreenView LastCreated { get; private set; }

        public SingleViewSplashScreenView()
        {
            LastCreated = this;
        }
    }
}
