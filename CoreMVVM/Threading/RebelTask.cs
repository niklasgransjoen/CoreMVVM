using CoreMVVM.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public RebelTask(Action action)
        {
            Task = new Task(action);
        }

        public RebelTask(Action<object> action, object state)
        {
            Task = new Task(action, state);
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

        public static RebelTask Delay(int millisecondsDelay)
        {
            Task result = Task.Delay(millisecondsDelay);

            return new RebelTask(result);
        }

        public static RebelTask Delay(TimeSpan delay)
        {
            Task result = Task.Delay(delay);
            return new RebelTask(result);
        }

        public static RebelTask Run(Action action)
        {
            Task result = Task.Run(action);
            return new RebelTask(result);
        }

        public static RebelTask<TResult> Run<TResult>(Func<TResult> action)
        {
            Task<TResult> result = Task.Run(action);

            return new RebelTask<TResult>(result);
        }

        public static RebelTask Run(Func<RebelTask> action)
        {
            Task result = Task.Run(() => action().Task);
            return new RebelTask(result);
        }

        public static RebelTask<TResult> Run<TResult>(Func<RebelTask<TResult>> action)
        {
            Task<TResult> result = Task.Run(() => action().Task);
            return new RebelTask<TResult>(result);
        }

        public static RebelTask Run(Func<Task> action)
        {
            Task result = Task.Run(action);
            return new RebelTask(result);
        }

        public static RebelTask<TResult> Run<TResult>(Func<Task<TResult>> action)
        {
            Task<TResult> result = Task.Run(action);
            return new RebelTask<TResult>(result);
        }

        public static RebelTask WhenAll(IEnumerable<RebelTask> tasks)
        {
            IEnumerable<Task> wrappedTasks = tasks.Select(t => t.Task);
            Task result = Task.WhenAll(wrappedTasks);

            return new RebelTask(result);
        }

        public static RebelTask<TResult[]> WhenAll<TResult>(IEnumerable<RebelTask<TResult>> tasks)
        {
            var wrappedTasks = tasks
                .Select(task => (RebelTask)task)
                .Select(t => t.Task)
                .Cast<Task<TResult>>();

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