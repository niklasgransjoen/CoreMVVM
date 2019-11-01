using CoreMVVM.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace CoreMVVM.Threading
{
    /// <summary>
    /// A task that does not continue on the captured context as default.
    /// </summary>
    [AsyncMethodBuilder(typeof(RebelTaskMethodBuilder))]
    public readonly struct RebelTask : IEquatable<RebelTask>
    {
        private readonly Task _task;

        #region Constructors

        public static RebelTask CompletedTask { get; } = new RebelTask(Task.CompletedTask);

        public RebelTask(Task task)
        {
            _task = task;
        }

        public static RebelTask<TResult> FromResult<TResult>(TResult result)
        {
            return new RebelTask<TResult>(result);
        }

        [DebuggerStepThrough]
        public static RebelTask Run(Func<Task> task)
        {
            return task();
        }

        [DebuggerStepThrough]
        public static RebelTask<TResult> Run<TResult>(Func<Task<TResult>> task)
        {
            return task();
        }

        [DebuggerStepThrough]
        public static RebelTask Delay(int millisecondsDelay)
        {
            Task result = Task.Delay(millisecondsDelay);

            return new RebelTask(result);
        }

        [DebuggerStepThrough]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Throw single aggregate exception")]
        public static RebelTask WhenAll(IEnumerable<RebelTask> tasks)
        {
            IEnumerable<Task> wrappedTasks = tasks.Select(t => t._task);
            Task result = Task.WhenAll(wrappedTasks);

            return new RebelTask(result);
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
            if (!(obj is RebelTask other))
                return false;

            return Equals(other);
        }

        public bool Equals(RebelTask other)
        {
            return _task == other._task;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = (hash * 7) + GetTaskOrComplete().GetHashCode();

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
    public readonly struct RebelTask<TResult> : INotifyCompletion, IEquatable<RebelTask<TResult>>
    {
        private readonly Task<TResult> _task;
        private readonly TResult _result;

        #region Constructors

        public RebelTask(Task<TResult> task)
        {
            _task = task;
            _result = default;
        }

        public RebelTask(TResult result)
        {
            _task = null;
            _result = result;
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
                if (_task is null)
                    return true;

                return _task.IsCompleted;
            }
        }

        public void OnCompleted(Action continuation)
        {
            if (IsCompleted)
                continuation();
            else
                _task.ConfigureAwait(false).GetAwaiter().OnCompleted(continuation);
        }

        /// <summary>
        /// Returns the result of this task.
        /// </summary>
        public TResult GetResult()
        {
            if (_task is null)
                return _result;

            if (_task.Exception != null)
            {
                Exception taskException = _task.Exception.InnerException;
                ExceptionDispatchInfo.Capture(taskException).Throw();
                throw taskException; // Is never called.
            }

            return _task.Result;
        }

        #endregion Awaiter

        #region Operators

        public override bool Equals(object obj)
        {
            if (!(obj is RebelTask<TResult> other))
                return false;

            return Equals(other);
        }

        public bool Equals(RebelTask<TResult> other)
        {
            if (_task != null && other._task != null)
                return _task == other._task;

            if (_task == null ^ other._task == null)
                return false;

            if (_result == null && other._result == null)
                return true;

            if (_result == null ^ other._result == null)
                return false;

            return _result.Equals(other._result);
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