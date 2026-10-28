using Avalonia.Threading;
using System;
using System.Diagnostics;
using System.Threading;

namespace Radical.Avalonia.Tests
{
    static class UIThread
    {
        /// <summary>
        /// Processes UI thread jobs until the given condition is met or the timeout expires.
        /// Work dispatched from background threads (e.g. broadcast messages) needs the
        /// UI thread to be pumped while the test is waiting.
        /// </summary>
        public static bool RunJobsUntil(Func<bool> condition, int timeoutMilliseconds = 5000)
        {
            var sw = Stopwatch.StartNew();
            while (!condition())
            {
                if (sw.ElapsedMilliseconds > timeoutMilliseconds)
                {
                    return false;
                }

                Dispatcher.UIThread.RunJobs();
                Thread.Sleep(10);
            }

            return true;
        }
    }
}
