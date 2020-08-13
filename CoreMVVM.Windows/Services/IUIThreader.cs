using CoreMVVM.IOC;
using CoreMVVM.Threading;
using System;

namespace CoreMVVM.Windows
{
    /// <summary>
    /// Implements functionality for executing on the UI thread.
    /// </summary>
    [FallbackImplementation(typeof(FallbackImplementations.UIThreader))]
    public interface IUIThreader
    {
        /// <summary>
        /// Checks if the current thread is the UI thread.
        /// </summary>
        bool IsUIThread();

        /// <summary>
        /// Runs the given action on the UI thread.
        /// </summary>
        /// <param name="action">The action to run on the UI thread. Not null.</param>
        /// <exception cref="ArgumentNullException">Action is null.</exception>
        void RunOnUIThread(Action action);

        /// <summary>
        /// Runs the given action async on the UI thread.
        /// </summary>
        /// <param name="action">The action to run on the UI thread. Not null.</param>
        /// <exception cref="ArgumentNullException">Action is null.</exception>
        RebelTask RunOnUIThreadAsync(Action action);

        /// <summary>
        /// Runs the given action on the UI thread.
        /// </summary>
        /// <param name="action">The action to run on the UI thread. Not null.</param>
        /// <exception cref="ArgumentNullException">Action is null.</exception>
        T RunOnUIThread<T>(Func<T> action);

        /// <summary>
        /// Runs the given action async on the UI thread.
        /// </summary>
        /// <param name="action">The action to run on the UI thread. Not null.</param>
        /// <exception cref="ArgumentNullException">Action is null.</exception>
        RebelTask<T> RunOnUIThreadAsync<T>(Func<T> action);

        /// <summary>
        /// Schedules the given action to run on the UI thread.
        /// </summary>
        /// <param name="action">The action to schedule for running on the UI thread. Not null.</param>
        /// <exception cref="ArgumentNullException">Action is null.</exception>
        /// <remarks>The action is not executed immediately. Instead, calls to this method are grouped together and executed on the UI thread.</remarks>
        void Schedule(Action action);
    }
}