﻿using CoreMVVM.Threading;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CoreMVVM.CompilerServices
{
#if NETCORE

    /// <summary>
    /// Task builder for the non-generic <see cref="RebelTask"/>.
    /// </summary>
    /// <remarks>This builder is a simple wrapper of the general task builder.</remarks>
    [DebuggerStepThrough]
    public sealed class RebelTaskMethodBuilder
    {
        private AsyncTaskMethodBuilder _builder = AsyncTaskMethodBuilder.Create();

        public RebelTaskMethodBuilder()
        {
        }

        public static RebelTaskMethodBuilder Create() => new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        public RebelTask Task
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(_builder.Task);
        }

        public void SetResult()
        {
            _builder.SetResult();
        }

        internal object ObjectIdForDebugger { get; } = Guid.NewGuid();
    }

    /// <summary>
    /// Task builder for the generic <see cref="RebelTask{TResult}"/>.
    /// </summary>
    /// <remarks>This builder is a simple wrapper of the general task builder.</remarks>
    [DebuggerStepThrough]
    public sealed class RebelTaskMethodBuilder<TResult>
    {
        private AsyncTaskMethodBuilder<TResult> _builder = AsyncTaskMethodBuilder<TResult>.Create();

        public RebelTaskMethodBuilder()
        {
        }

        public static RebelTaskMethodBuilder<TResult> Create() => new();

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

        public RebelTask<TResult> Task => new(_builder.Task);

        internal object ObjectIdForDebugger { get; } = Guid.NewGuid();
    }

#endif
}