using CoreMVVM.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace CoreMVVM.Threading
{
    /// <summary>
    /// A task that does not continue on the captured context as default.
    /// </summary>
#if NETCORE

    [AsyncMethodBuilder(typeof(RebelTaskMethodBuilder))]
#endif
    [DebuggerStepThrough]
    public readonly struct RebelTask : IEquatable<RebelTask>
    {
#if NETCORE || NETSTANDARD
        public static RebelTask CompletedTask { get; } = new RebelTask(Task.CompletedTask);
#else
        public static RebelTask CompletedTask { get; } = new RebelTask(Task.FromResult<object>(null));
#endif

        #region Constructors

        public RebelTask(Task task)
        {
            Task = task;
        }

        public RebelTask(Action action, CancellationToken cancellationToken = default)
        {
            Task = new Task(action, cancellationToken);
        }

        public RebelTask(Action<object> action, object state, CancellationToken cancellationToken = default)
        {
            Task = new Task(action, state, cancellationToken);
        }

        public static RebelTask<TResult> FromResult<TResult>(TResult result)
        {
            return new RebelTask<TResult>(result);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the task object of this RebelTask.
        /// </summary>
        public Task Task { get; }

        #endregion Properties

        /// <summary>
        /// Starts the task on the current task scheduler.
        /// </summary>
        public void Start()
        {
            Task.Start();
        }

        public void Start(TaskScheduler taskScheduler)
        {
            Task.Start(taskScheduler);
        }

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
#if NETCORE
            return Task ?? Task.CompletedTask;
#else
            return Task ?? CompletedTask.Task;
#endif
        }

        #region Static utilities

        public static RebelTask Delay(int millisecondsDelay, CancellationToken cancellationToken = default)
        {
            Task result = Task.Delay(millisecondsDelay, cancellationToken);
            return new RebelTask(result);
        }

        public static RebelTask Delay(TimeSpan delay, CancellationToken cancellationToken = default)
        {
            Task result = Task.Delay(delay, cancellationToken);
            return new RebelTask(result);
        }

        public static RebelTask Run(Action action, CancellationToken cancellationToken = default)
        {
            Task result = Task.Run(action, cancellationToken);
            return new RebelTask(result);
        }

        public static RebelTask<TResult> Run<TResult>(Func<TResult> action, CancellationToken cancellationToken = default)
        {
            Task<TResult> result = Task.Run(action, cancellationToken);
            return new RebelTask<TResult>(result);
        }

        public static RebelTask Run(Func<RebelTask> action, CancellationToken cancellationToken = default)
        {
            Task result = Task.Run(() => action().Task, cancellationToken);
            return new RebelTask(result);
        }

        public static RebelTask<TResult> Run<TResult>(Func<RebelTask<TResult>> action, CancellationToken cancellationToken = default)
        {
            Task<TResult> result = Task.Run(() => action().Task, cancellationToken);
            return new RebelTask<TResult>(result);
        }

        public static RebelTask Run(Func<Task> action, CancellationToken cancellationToken = default)
        {
            Task result = Task.Run(action, cancellationToken);
            return new RebelTask(result);
        }

        public static RebelTask<TResult> Run<TResult>(Func<Task<TResult>> action, CancellationToken cancellationToken = default)
        {
            Task<TResult> result = Task.Run(action, cancellationToken);
            return new RebelTask<TResult>(result);
        }

        public static RebelTask WhenAll(params RebelTask[] tasks) => WhenAll((IReadOnlyCollection<RebelTask>)tasks);

        public static RebelTask WhenAll(IReadOnlyCollection<RebelTask> tasks)
        {
            if (tasks.Count == 0)
                return CompletedTask;

            return WhenAll(tasks.AsEnumerable());
        }

        public static RebelTask WhenAll(IEnumerable<RebelTask> tasks)
        {
            IEnumerable<Task> wrappedTasks = tasks.Select(t => t.Task);
            Task result = Task.WhenAll(wrappedTasks);

            return new RebelTask(result);
        }

        public static RebelTask<TResult[]> WhenAll<TResult>(params RebelTask<TResult>[] tasks) => WhenAll((IReadOnlyCollection<RebelTask<TResult>>)tasks);

        public static RebelTask<TResult[]> WhenAll<TResult>(IReadOnlyCollection<RebelTask<TResult>> tasks)
        {
#if NET45
            if (tasks.Count == 0)
                return FromResult(new TResult[0]);
#else
            if (tasks.Count == 0)
                return FromResult(Array.Empty<TResult>());
#endif

            return WhenAll(tasks.AsEnumerable());
        }

        public static RebelTask<TResult[]> WhenAll<TResult>(IEnumerable<RebelTask<TResult>> tasks)
        {
            var wrappedTasks = tasks
                .Select(task => task.Task);

            var result = Task.WhenAll(wrappedTasks);

            return new RebelTask<TResult[]>(result);
        }

        #endregion Static utilities

        #region Operators

        public override bool Equals(object obj)
        {
            if (!(obj is RebelTask other))
                return false;

            return Equals(other);
        }

        public bool Equals(RebelTask other)
        {
            return Task == other.Task;
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
}