using System;
using System.Windows;
using System.Windows.Threading;

namespace CoreMVVM.Windows.Threading
{
    /// <summary>
    /// A tool for switching between threads.
    /// </summary>
    public static class TaskMaster
    {
        /// <summary>
        /// Switches to the UI thread, by using the dispatcher of the current application.
        /// </summary>
        /// <exception cref="InvalidOperationException"><see cref="Application.Current"/> is null.</exception>
        public static DispatcherThreadSwitcher AwaitUIThread()
        {
            if (Application.Current is null)
                throw new InvalidOperationException("Cannot await UI thread when Application.Current is null");

            return AwaitThread(Application.Current.Dispatcher);
        }

        /// <summary>
        /// Switches to the thread of the given dispatcher.
        /// </summary>
        /// <param name="dispatcher">The dispatcher to switch to.</param>
        public static DispatcherThreadSwitcher AwaitThread(Dispatcher dispatcher)
        {
            return new DispatcherThreadSwitcher(dispatcher);
        }

        /// <summary>
        /// Switches to a new background thread.
        /// </summary>
        public static ThreadPoolThreadSwitcher AwaitBackgroundThread()
        {
            return new ThreadPoolThreadSwitcher();
        }
    }
}