using CoreMVVM.CompilerServices;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CoreMVVM.Threading
{
    /// <summary>
    /// A task that does not continue on the captured context as default.
    /// </summary>
    [AsyncMethodBuilder(typeof(RebelTaskMethodBuilder))]
    public readonly struct RebelTask
    {
        private readonly Task _task;

        #region Constructors

        public static RebelTask CompletedTask => new RebelTask(Task.CompletedTask);

        public RebelTask(Task task)
        {
            _task = task;
        }

        public static RebelTask<TResult> FromResult<TResult>(TResult result)
        {
            return new RebelTask<TResult>(result);
        }

        #endregion Constructors

        /// <summary>
        /// Returns a task configured to continue on the captured context.
        /// </summary>
        public ConfiguredTaskAwaitable ContinueOnCapturedContext()
        {
            Task task = GetTaskOrComplete();
            return task.ConfigureAwait(true);
        }

        /// <summary>
        /// Returns the awaiter of this task, configured to not continue on the captured context.
        /// </summary>
        public ConfiguredTaskAwaitable.ConfiguredTaskAwaiter GetAwaiter()
        {
            Task task = GetTaskOrComplete();
            return task.ConfigureAwait(false).GetAwaiter();
        }

        private Task GetTaskOrComplete()
        {
            return _task ?? Task.CompletedTask;
        }

        #region Operators

        public override bool Equals(object obj)
        {
            if (!(obj is RebelTask task))
                return false;

            return _task == task._task;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = (hash * 7) + _task?.GetHashCode() ?? 13;

            return hash;
        }

        public static bool operator ==(RebelTask left, RebelTask right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RebelTask left, RebelTask right)
        {
            return !left.Equals(right);
        }

        public static implicit operator RebelTask(Task task)
        {
            return new RebelTask(task);
        }

        #endregion Operators
    }

    /// <summary>
    /// A task that does not continue on the captured context as default.
    /// </summary>
    [AsyncMethodBuilder(typeof(RebelTaskMethodBuilder<>))]
    public readonly struct RebelTask<TResult> : INotifyCompletion
    {
        private readonly Task<TResult> _task;
        private readonly TResult _result;

        #region Constructors

        public RebelTask(Task<TResult> task)
        {
            if (task.IsCompleted)
            {
                _task = null;
                _result = task.Result;
            }
            else
            {
                _task = task;
                _result = default;
            }
        }

        public RebelTask(TResult result)
        {
            _task = null;
            _result = result;
        }

        public static RebelTask<T> FromResult<T>(T result)
        {
            return new RebelTask<T>(result);
        }

        #endregion Constructors

        /// <summary>
        /// Returns a task configured to continue on the captured context.
        /// </summary>
        public ConfiguredTaskAwaitable<TResult> ContinueOnCapturedContext()
        {
            if (_task is null)
                return Task.FromResult(_result).ConfigureAwait(true);

            return _task.ConfigureAwait(true);
        }

        /// <summary>
        /// Returns the awaiter of this task, configured to not continue on the captured context.
        /// </summary>
        public RebelTask<TResult> GetAwaiter() => this;

        #region Awaiter

        /// <summary>
        /// Gets a value indicating if this task has completed.
        /// </summary>
        public bool IsCompleted
        {
            get
            {
                if (_result != null)
                    return true;

                return _task.IsCompleted;
            }
        }

        public void OnCompleted(Action continuation)
        {
            if (_result != null)
                continuation();
            else
                _task.ConfigureAwait(false).GetAwaiter().OnCompleted(continuation);
        }

        /// <summary>
        /// Returns the result of this task.
        /// </summary>
        public TResult GetResult()
        {
            if (_result != null)
                return _result;

            return _task.Result;
        }

        #endregion Awaiter

        #region Operators

        public override bool Equals(object obj)
        {
            if (!(obj is RebelTask<TResult> task))
                return false;

            if (_result == null && task._result == null)
            {
                return _task == task._task;
            }

            if (_result == null ^ _result == null)
                return false;

            return _result.Equals(_result);
        }

        public override int GetHashCode()
        {
            int hash = 39;
            hash = (hash * 7) + _result?.GetHashCode() ?? 13;
            hash = (hash * 7) + _task?.GetHashCode() ?? 13;

            return hash;
        }

        public static bool operator ==(RebelTask<TResult> left, RebelTask<TResult> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RebelTask<TResult> left, RebelTask<TResult> right)
        {
            return !left.Equals(right);
        }

        public static implicit operator RebelTask<TResult>(Task<TResult> task)
        {
            return new RebelTask<TResult>(task);
        }

        #endregion Operators
    }
}