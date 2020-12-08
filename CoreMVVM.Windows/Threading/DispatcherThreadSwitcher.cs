#pragma warning disable CA1822 // Mark members as static

using System;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace CoreMVVM.Windows.Threading
{
    /// <summary>
    /// A task-like structure used to switch context to a specific dispatcher.
    /// </summary>
    public struct DispatcherThreadSwitcher : INotifyCompletion, IEquatable<DispatcherThreadSwitcher>
    {
        private readonly Dispatcher _dispatcher;

        public DispatcherThreadSwitcher(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public DispatcherThreadSwitcher GetAwaiter() => this;

        /// <exception cref="InvalidOperationException">Struct created with no dispatcher.</exception>
        public bool IsCompleted
        {
            get
            {
                ThrowIfDispatcherIsNull();
                return _dispatcher.CheckAccess();
            }
        }

        public void GetResult()
        { }

        /// <exception cref="InvalidOperationException">Struct created with no dispatcher.</exception>
        public void OnCompleted(Action continuation)
        {
            ThrowIfDispatcherIsNull();
            _dispatcher.BeginInvoke(continuation);
        }

        private void ThrowIfDispatcherIsNull()
        {
            if (_dispatcher == null)
                throw new InvalidOperationException($"Call to '{nameof(OnCompleted)}' is illegal on structure without a dispatcher.");
        }

        #region Operators

        public override bool Equals(object? obj)
        {
            return obj is DispatcherThreadSwitcher other && Equals(other);
        }

        public bool Equals(DispatcherThreadSwitcher other)
        {
            return _dispatcher == other._dispatcher;
        }

        public override int GetHashCode()
        {
            return _dispatcher?.GetHashCode() ?? -1;
        }

        public static bool operator ==(DispatcherThreadSwitcher left, DispatcherThreadSwitcher right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DispatcherThreadSwitcher left, DispatcherThreadSwitcher right)
        {
            return !(left == right);
        }

        #endregion Operators
    }
}