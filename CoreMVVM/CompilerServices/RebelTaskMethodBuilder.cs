using CoreMVVM.Threading;
using System;
using System.Runtime.CompilerServices;

namespace CoreMVVM.CompilerServices
{
#if NETCORE

    /// <summary>
    /// Task builder for the non-generic <see cref="RebelTask"/>.
    /// </summary>
    /// <remarks>This builder is a simple wrapper of the general task builder.</remarks>
    public sealed class RebelTaskMethodBuilder
    {
        private AsyncTaskMethodBuilder _builder = AsyncTaskMethodBuilder.Create();

        public RebelTaskMethodBuilder()
        {
        }

        public static RebelTaskMethodBuilder Create()
        {
            return new RebelTaskMethodBuilder();
        }

        public void SetResult()
        {
            _builder.SetResult();
        }

        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
        {
            _builder.Start(ref stateMachine);
        }

        public void SetException(Exception e)
        {
            _builder.SetException(e);
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            _builder.AwaitOnCompleted(ref awaiter, ref stateMachine);
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            _builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            _builder.SetStateMachine(stateMachine);
        }

        public RebelTask Task => new RebelTask(_builder.Task);
    }

    /// <summary>
    /// Task builder for the generic <see cref="RebelTask{TResult}"/>.
    /// </summary>
    /// <remarks>This builder is a simple wrapper of the general task builder.</remarks>
    public sealed class RebelTaskMethodBuilder<TResult>
    {
        private AsyncTaskMethodBuilder<TResult> _builder = AsyncTaskMethodBuilder<TResult>.Create();

        public RebelTaskMethodBuilder()
        {
        }

        public static RebelTaskMethodBuilder<TResult> Create()
        {
            return new RebelTaskMethodBuilder<TResult>();
        }

        public void SetResult(TResult result)
        {
            _builder.SetResult(result);
        }

        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
        {
            _builder.Start(ref stateMachine);
        }

        public void SetException(Exception e)
        {
            _builder.SetException(e);
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            _builder.AwaitOnCompleted(ref awaiter, ref stateMachine);
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            _builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            _builder.SetStateMachine(stateMachine);
        }

        public RebelTask<TResult> Task => new RebelTask<TResult>(_builder.Task);
    }

#endif
}