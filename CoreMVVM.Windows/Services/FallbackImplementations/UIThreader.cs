using CoreMVVM.Threading;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace CoreMVVM.Windows.FallbackImplementations
{
    /// <summary>
    /// Default implementation of the <see cref="IUIThreader"/>.
    /// </summary>
    public class UIThreader : IUIThreader, IDisposable
    {
        private bool _isDisposed;
        private readonly object _disposeLock = new object();

        private readonly Timer _timer = new Timer(interval: 1);
        private readonly List<Action> _scheduledActions = new List<Action>();
        private readonly object _scheduleLock = new object();

        #region Construct & Dispose

        public UIThreader()
        {
            _timer.Elapsed += ScheduleTimerElapsed;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if (_isDisposed)
                return;

            lock (_disposeLock)
            {
                if (_isDisposed)
                    return;

                _isDisposed = true;
            }

            _timer?.Dispose();
        }

        #endregion Construct & Dispose

        public virtual bool IsUIThread()
        {
            ThrowIfApplicationIsNull();
            return Application.Current.CheckAccess();
        }

        public void RunOnUIThread(Action action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            ThrowIfApplicationIsNull();
            Application.Current.Dispatcher.Invoke(action);
        }

        public T RunOnUIThread<T>(Func<T> action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            ThrowIfApplicationIsNull();
            return Application.Current.Dispatcher.Invoke(action);
        }

        public RebelTask RunOnUIThreadAsync(Action action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            ThrowIfApplicationIsNull();
            return run(action);

            static async Task run(Action action)
            {
                await Application.Current.Dispatcher.InvokeAsync(action);
            }
        }

        public RebelTask<T> RunOnUIThreadAsync<T>(Func<T> action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            ThrowIfApplicationIsNull();
            return run(action);

            static async Task<T> run(Func<T> action)
            {
                return await Application.Current.Dispatcher.InvokeAsync(action);
            }
        }

        public void Schedule(Action action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            lock (_disposeLock)
            {
                if (_isDisposed)
                    throw new ObjectDisposedException(GetType().ToString());
            }

            lock (_scheduleLock)
            {
                _timer.Start();
                _scheduledActions.Add(action);
            }
        }

        private void ScheduleTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Action[] scheduledActionsCopy;
            lock (_scheduleLock)
            {
                _timer.Stop();

                scheduledActionsCopy = _scheduledActions.ToArray();
                _scheduledActions.Clear();
            }

            RunOnUIThread(() =>
            {
                foreach (var action in scheduledActionsCopy)
                {
                    action();
                }
            });
        }

        #region Helpers

        private void ThrowIfApplicationIsNull()
        {
            if (Application.Current is null)
                throw new InvalidOperationException($"Cannot use '{GetType()}' when System.Windows.Application.Current is null.");
        }

        #endregion Helpers
    }
}