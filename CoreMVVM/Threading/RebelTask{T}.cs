using CoreMVVM.CompilerServices;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace CoreMVVM.Threading
{
    /// <summary>
    /// A task that does not continue on the captured context as default.
    /// </summary>
#if NETCORE

    [AsyncMethodBuilder(typeof(RebelTaskMethodBuilder<>))]
#endif
    [DebuggerStepThrough]
    public readonly struct RebelTask<TResult> : INotifyCompletion, IEquatable<RebelTask<TResult>>
    {
        [AllowNull]
        private readonly TResult _result;

        #region Constructors

        public RebelTask(Task<TResult> task)
        {
            Task = task;
            _result = default;
        }

        public RebelTask(Func<TResult> function, CancellationToken cancellationToken = default)
        {
            Task = new Task<TResult>(function, cancellationToken);
            _result = default;
        }

        public RebelTask(Func<object, TResult> function, object state, CancellationToken cancellationToken = default)
        {
            Task = new Task<TResult>(function, state, cancellationToken);
            _result = default;
        }

        public RebelTask(TResult result)
        {
            Task = null;
            _result = result;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the task object of this RebelTask.
        /// </summary>
        public Task<TResult>? Task { get; }

        #endregion Properties

        /// <summary>
        /// Starts the task on the current task scheduler.
        /// </summary>
        public void Start()
        {
            Task?.Start();
        }

        public void Start(TaskScheduler taskScheduler)
        {
            Task?.Start(taskScheduler);
        }

        /// <summary>
        /// Returns a task configured to continue on the captured context.
        /// </summary>
        public ConfiguredTaskAwaitable<TResult> ContinueOnCapturedContext()
        {
            if (Task is null)
                return System.Threading.Tasks.Task.FromResult(_result).ConfigureAwait(true);

            return Task.ConfigureAwait(true);
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
                if (Task is null)
                    return true;

                return Task.IsCompleted;
            }
        }

        public void OnCompleted(Action continuation)
        {
            if (continuation is null)
                throw new ArgumentNullException(nameof(continuation));

            if (IsCompleted)
                continuation();
            else
                Task!.ConfigureAwait(false).GetAwaiter().OnCompleted(continuation);
        }

        /// <summary>
        /// Returns the result of this task.
        /// </summary>
        public TResult GetResult()
        {
            if (Task is null)
                return _result;

            if (Task.IsCanceled)
            {
                throw new TaskCanceledException(Task);
            }

            if (Task.Exception != null)
            {
                Exception taskException = Task.Exception.InnerException;
                ExceptionDispatchInfo.Capture(taskException).Throw();
                throw taskException; // Is never called.
            }

            return Task.Result;
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
            if (Task is null && other.Task is null)
                return Equals(_result, other._result);
            else
                return Equals(Task, other.Task);
        }

        public override int GetHashCode()
        {
            int hash = 39;
            hash = (hash * 7) + _result?.GetHashCode() ?? 13;
            hash = (hash * 7) + Task?.GetHashCode() ?? 13;

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

        public static implicit operator RebelTask(RebelTask<TResult> rebelTask)
        {
            if (rebelTask.Task is null)
                return new RebelTask(System.Threading.Tasks.Task.FromResult(rebelTask._result));

            return new RebelTask(rebelTask.Task);
        }

        #endregion Operators
    }
}