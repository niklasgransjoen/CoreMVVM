using CoreMVVM.Windows.Threading;
using NUnit.Framework;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CoreMVVM.Windows.Tests
{
    [TestFixture]
    public sealed class TaskMasterTests
    {
        [Test]
        public async Task SwitchesThread()
        {
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

            Task threadCheck = PerformDispatcherSwitch(dispatcher);

            // Make sure dispatcher is invoked before awaiting thread switch task.
            // This is ugly, but works (apparently).
            Assert.True(dispatcher.CheckAccess());
            for (int i = 0; i < 10_000; i++)
                DispatcherUtil.DoEvents();

            await threadCheck;
        }

        private async Task PerformDispatcherSwitch(Dispatcher dispatcher)
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
        public static class DispatcherUtil
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public static void DoEvents()
            {
                DispatcherFrame frame = new DispatcherFrame();
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
                Dispatcher.PushFrame(frame);
            }

            private static object ExitFrame(object frame)
            {
                ((DispatcherFrame)frame).Continue = false;
                return null;
            }
        }
    }
}