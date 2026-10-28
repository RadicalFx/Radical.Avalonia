using Avalonia.Controls;

namespace Radical.Avalonia.Tests.Boot.Presentation
{
    public class TestSplashScreenView : Window
    {
        public static TestSplashScreenView LastCreated { get; private set; }

        public TestSplashScreenView()
        {
            LastCreated = this;
        }
    }
}
