using Avalonia;
using Avalonia.Headless;
using Avalonia.Themes.Simple;
using Radical.Avalonia.Tests;

[assembly: AvaloniaTestApplication(typeof(TestApplication))]

namespace Radical.Avalonia.Tests
{
    public class TestApplication : Application
    {
        public override void Initialize()
        {
            // Templated controls, such as ScrollViewer and ListBox, need a theme to be laid out.
            Styles.Add(new SimpleTheme());
        }

        public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<TestApplication>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions());
    }
}
