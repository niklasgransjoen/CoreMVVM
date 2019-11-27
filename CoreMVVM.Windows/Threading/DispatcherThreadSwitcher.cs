using System;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace CoreMVVM.Windows.Threading
{
    /// <summary>
    /// A task-like structure used to switch context to a specific dispatcher.
    /// </summary>
    public struct DispatcherThreadSwitcher : INotifyCompletion
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
    }
}