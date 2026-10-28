using Avalonia;
using Avalonia.Headless;
using Radical.Avalonia.Tests;

[assembly: AvaloniaTestApplication(typeof(TestApplication))]

namespace Radical.Avalonia.Tests
{
    public class TestApplication : Application
    {
        public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<TestApplication>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions());
    }
}
