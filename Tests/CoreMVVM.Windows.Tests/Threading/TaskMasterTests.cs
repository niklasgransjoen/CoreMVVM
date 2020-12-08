using System.Threading.Tasks;
using System.Windows.Threading;
using Xunit;

namespace CoreMVVM.Windows.Threading.Tests
{
    public sealed class TaskMasterTests
    {
        [Fact]
        public async Task SwitchesThread()
        {
            var dispatcher = Dispatcher.CurrentDispatcher;

            var threadCheck = PerformDispatcherSwitch(dispatcher);

            // Make sure dispatcher is invoked before awaiting thread switch task.
            // This is ugly, but works (apparently).
            Assert.True(dispatcher.CheckAccess());
            for (var i = 0; i < 10_000; i++)
                DispatcherUtil.DoEvents();

            await threadCheck;
        }

        private static async Task PerformDispatcherSwitch(Dispatcher dispatcher)
        {
            Assert.True(dispatcher.CheckAccess());

            // Switch thread.
            await TaskMaster.AwaitBackgroundThread();
            Assert.False(dispatcher.CheckAccess());

            // Switch back to UI thread.
            await TaskMaster.AwaitThread(dispatcher);
            Assert.True(dispatcher.CheckAccess());
        }

        /// <summary>
        /// Utility for executing the dispatcher queue.
        /// </summary>
        private static class DispatcherUtil
        {
            public static void DoEvents()
            {
                var frame = new DispatcherFrame();
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
                Dispatcher.PushFrame(frame);
            }

            private static object? ExitFrame(object frame)
            {
                ((DispatcherFrame)frame).Continue = false;
                return null;
            }
        }
    }
}