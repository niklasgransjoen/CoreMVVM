using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace CoreMVVM.Windows.Threading
{
    /// <summary>
    /// A task-like structure used to switch context to a new thread.
    /// </summary>
    public struct ThreadPoolThreadSwitcher : INotifyCompletion
    {
        public ThreadPoolThreadSwitcher GetAwaiter() => this;

        public bool IsCompleted => false;

        public void GetResult()
        { }

        public void OnCompleted(Action continuation) =>
            ThreadPool.QueueUserWorkItem(_ => continuation());
    }
}