using Avalonia.Headless;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace Radical.Avalonia.Tests
{
    /// <summary>
    /// Runs the test method on the Avalonia UI thread of a headless
    /// application session, the Avalonia counterpart of the WPF
    /// STA and shared application test attributes.
    /// </summary>
    public class AvaloniaTestMethodAttribute : TestMethodAttribute
    {
        public override Task<TestResult[]> ExecuteAsync(ITestMethod testMethod)
        {
            var session = HeadlessUnitTestSession.GetOrStartForAssembly(typeof(AvaloniaTestMethodAttribute).Assembly);

            // Capture the execution context so that MSTest 4.2+ AsyncLocal values
            // (e.g. TestContext.Current) are visible on the Avalonia UI thread.
            var capturedContext = ExecutionContext.Capture();

            return session.Dispatch(() =>
            {
                Task<TestResult[]> result = null;
                ExecutionContext.Run(capturedContext, _ => result = base.ExecuteAsync(testMethod), null);

                return result;
            }, CancellationToken.None);
        }
    }
}
