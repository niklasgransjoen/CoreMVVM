using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace CoreMVVM.Windows.Threading
{
    /// <summary>
    /// A task-like structure used to switch context to a new thread.
    /// </summary>
    public struct ThreadPoolThreadSwitcher : INotifyCompletion, IEquatable<ThreadPoolThreadSwitcher>
    {
        public ThreadPoolThreadSwitcher GetAwaiter() => this;

        public bool IsCompleted => false;

        public void GetResult()
        { }

        public void OnCompleted(Action continuation) =>
            ThreadPool.QueueUserWorkItem(_ => continuation());

        #region Operators

        public override bool Equals(object? obj)
        {
            return obj is ThreadPoolThreadSwitcher other && Equals(other);
        }

        public bool Equals(ThreadPoolThreadSwitcher other)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public static bool operator ==(ThreadPoolThreadSwitcher left, ThreadPoolThreadSwitcher right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ThreadPoolThreadSwitcher left, ThreadPoolThreadSwitcher right)
        {
            return !(left == right);
        }

        #endregion Operators
    }
}